using Azure.Core;
using Mapster;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NotificationService.Models;
using SeeviceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.Common;
using ServiceProvider_BLL.Dtos.OrderDto;
using ServiceProvider_BLL.Dtos.OrderProductDto;
using ServiceProvider_BLL.Dtos.PaymentDto;
using ServiceProvider_BLL.Dtos.ReviewDto;
using ServiceProvider_BLL.Dtos.ShippingDto;
using ServiceProvider_BLL.Errors;
using ServiceProvider_BLL.Interfaces;
using ServiceProvider_DAL.Data;
using ServiceProvider_DAL.Entities;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.WebRequestMethods;
using Shipping = ServiceProvider_DAL.Entities.Shipping;

namespace ServiceProvider_BLL.Reposatories
{
    public class OrderRepository : BaseRepository<Order> , IOrderRepository
    {
        private readonly AppDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        private const int OtpLength = 6;                     // طول الرمز
        private static readonly TimeSpan OtpTtl = TimeSpan.FromMinutes(10); // مدة 

        public OrderRepository(AppDbContext context , IPublishEndpoint publishEndpoint) : base(context)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<Result<OrderResponseV2>> GetOrderAsync(int orderId, CancellationToken cancellationToken = default)
        {
            var order = await _context.Orders!
                     .Include(o => o.OrderProducts)
                     .ThenInclude(op => op.Product)
                     .Include(o => o.Payment)
                     .Include(o => o.Shippings)
                     .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken: cancellationToken);

            if (order == null)
                return Result.Failure<OrderResponseV2>(OrderErrors.OrderNotFound);


            var response = new OrderResponseV2(
                 order.Id,
                 order.TotalAmount,
                 order.OrderDate,
                 order.Status.ToString(),
                 order.OrderProducts.Select(op => new OrderProductResponse(
                     op.ProductId,
                     op.Product.NameEn,
                     op.Product.NameAr,
                     op.Product.Price,
                     op.Quantity
                 )).ToList(),
                 order.OrderProducts
                  .GroupBy(op => op.Product.Vendor.BusinessName)
                  .Select(g => new VendorSummaryResponse(
                       g.Key ?? "Unknown Vendor",
                       g.Sum(op => op.Product.Price * op.Quantity),
                       g.Sum(op => op.Quantity),
                       g.Select(op => new VendorOrderItemResponse(
                           op.ProductId,
                           op.Product.MainImageUrl!,
                           op.Product.NameEn,
                           op.Product.NameAr,
                           op.Product.Price,
                           op.Quantity
                       )).ToList()
                  )).ToList(),
                 new PaymentResponse(
                     order.Payment.TotalAmount,
                     order.Payment.Status.ToString(),
                     order.Payment.TransactionDate
                 )
            );

            return Result.Success(response);
        }

        public async Task<Result<PaginatedList<OrderResponse>>> GetAllOrderAsync(RequestFilter request, CancellationToken cancellationToken = default)
        {
            var query =  _context.Orders!
                     .Include(o => o.User)
                     .Include(o => o.OrderProducts)
                     .ThenInclude(op => op.Product)
                     .ThenInclude(p => p.Vendor)
                     .AsNoTracking();

            if (!query.Any())
                return Result.Failure<PaginatedList<OrderResponse>>(OrderErrors.OrderNotFound);

            if (!string.IsNullOrEmpty(request.SearchValue))
            {
                var searchTerm = $"%{request.SearchValue.ToLower()}%";
                query = query.Where(x =>
                    EF.Functions.Like(x.User.FullName.ToLower(), searchTerm) ||
                    EF.Functions.Like(x.Id.ToString() , searchTerm) ||
                    EF.Functions.Like(x.OrderProducts.Select(o => o.Product.Vendor.FullName).SingleOrDefault()!.ToLower(), searchTerm)
                );
            }

            if (!string.IsNullOrEmpty(request.SortColumn))
            {
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");
            }

            if (request.BusinessTypes != null && request.BusinessTypes.Any())
            {
                query = query.Where(x =>
                request.BusinessTypes.Any(bt =>
                    x.OrderProducts.First().Product.Vendor.BusinessType != null &&
                    x.OrderProducts.First().Product.Vendor.BusinessType.ToLower().Contains(bt.ToLower())
                ));
            }

            if (request.Statuses != null && request.Statuses.Any())
            {
                var statusEnums = request.Statuses
                 .Select(s => Enum.Parse<OrderStatus>(s, ignoreCase: true))
                 .ToList();

                query = query.Where(x => statusEnums.Contains(x.Status));
            }

            if (request.DateFilter != null && request.DateFilter.HasValue)
            {
                query = query.Where(x => DateOnly.FromDateTime(x.OrderDate) == request.DateFilter.Value);
            }

            var source =  query.Select(o => new OrderResponse(
                o.Id,
                o.ApplicationUserId,
                o.User.FullName,
                o.OrderProducts.Select(x => new VendorOrderResponse(
                    x.Product.Vendor.Id,
                    x.Product.Vendor.FullName,
                    x.Product.Vendor.BusinessName!,
                    x.Product.Vendor.BusinessType
                    )).Distinct().ToList(),
                o.TotalAmount,
                o.OrderDate,
                o.Status.ToString()
                ));

            var orders = await PaginatedList<OrderResponse>.CreateAsync(source, request.PageNumer, request.PageSize, cancellationToken);
            return Result.Success(orders);
        }

        public async Task<Result<PaginatedList<VendorOrderDto>>> GetUserOrdersAsync(string userId,RequestFilter request , CancellationToken cancellationToken = default)
        {
            var query = _context.Orders!
                .Where(o => o.ApplicationUserId == userId)
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .ThenInclude(p => p.Vendor)
                .Include(o => o.Payment)
                .OrderByDescending(o => o.OrderDate)
                .AsNoTracking();



            // Apply sorting if specified
            if (!string.IsNullOrEmpty(request.SortColumn))
            {
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");
            }



            if (request.Statuses != null && request.Statuses.Any())
            {
                var statusEnums = request.Statuses
                 .Select(s => Enum.Parse<OrderStatus>(s, ignoreCase: true))
                 .ToList();

                query = query.Where(x => statusEnums.Contains(x.Status));
            }


            var projectedQuery = query
                .SelectMany(order => order.OrderProducts
                    .GroupBy(op => op.Product!.VendorId)
                    .Select(g => new VendorOrderDto
                    (
                       order.Id,
                       g.First().Product!.Vendor!.Id,
                       g.First().Product!.Vendor!.BusinessName!,
                       g.First().Product!.Vendor!.ProfilePictureUrl!,
                       order.Status.ToString(),
                       order.OrderDate,
                       g.Sum(op => op.Quantity),
                       g.Sum(op => op.Product!.Price * op.Quantity)
                    )));
                

            //var projectedQuery = query.Select(o => new OrderResponseV2(
            //    o.Id,
            //    //o.TotalAmount,
            //    o.OrderDate,
            //    o.Status.ToString(),
            //    //o.OrderProducts.Select(op => new OrderProductResponse(
            //    //    op.ProductId,
            //    //    op.Product.NameEn,
            //    //    op.Product.NameAr,
            //    //    op.Product.Price,
            //    //    op.Quantity
            //    //)).ToList(),
            //    o.OrderProducts
            //      .GroupBy(op => op.Product.Vendor.BusinessName)
            //      .Select(g => new VendorSummaryResponse(
            //           g.Key ?? "Unknown Vendor",
            //           g.Sum(op => op.Product.Price * op.Quantity),
            //           g.Sum(op => op.Quantity),
            //           g.Select(op => new VendorOrderItemResponse(
            //               op.ProductId,
            //               op.Product.NameEn,
            //               op.Product.NameAr,
            //               op.Product.Price,
            //               op.Quantity
            //           )).ToList()
            //      )).ToList(),
            //    new PaymentResponse(
            //        o.Payment.TotalAmount,
            //        o.Payment.Status.ToString(),
            //        o.Payment.TransactionDate
            //        //o.Payment.PaymentMethodType
            //    ),
            //    o.Shipping != null ? new ShippingResponse(
            //        o.Shipping.Status,
            //        o.Shipping.EstimatedDeliveryDate
            //    ) : null
            //));

            var orders = await PaginatedList<VendorOrderDto>.CreateAsync(
            projectedQuery,
                request.PageNumer,
                request.PageSize,
                cancellationToken
            );

            return orders.Items.Any()
                ? Result.Success(orders)
                : Result.Failure<PaginatedList<VendorOrderDto>>(OrderErrors.OrderNotFound);
        }

        public async Task<Result<IEnumerable<RecentOrderResponse>>> GetTopFiveRecentOrdersAsync( string vendorId, int count = 5,CancellationToken cancellationToken = default)
        {
            var orders = await _context.Orders!
                .Where(o => o.OrderProducts.Any(op => op.Product.VendorId == vendorId))
                .OrderByDescending(o => o.OrderDate)
                .Take(count)
                .Select(o => new RecentOrderResponse(
                    o.Id,
                    o.OrderDate,
                    o.TotalAmount,
                    o.Status.ToString(),
                    o.User.FullName,
                    o.OrderProducts.Select(op => new OrderItemResponse(
                        op.Product.NameEn,
                        op.Product.NameAr,
                        op.Quantity,
                        op.Product.Price
                    )).ToList()
                ))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return orders.Any()
                ? Result.Success(orders.AsEnumerable())
                : Result.Failure<IEnumerable<RecentOrderResponse>>(OrderErrors.NoOrdersForThisVendor);
        }
        public async Task<Result<OrderResponseV2>> AddOrderAsync( string userId ,OrderRequest request, CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var cart = await _context.Carts!
                    .Include(c => c.CartProducts)
                    .ThenInclude(cp => cp.Product)
                    .FirstOrDefaultAsync(c => c.ApplicationUserId == userId, cancellationToken: cancellationToken);
                    

                if (cart == null || !cart.CartProducts.Any())
                    return Result.Failure<OrderResponseV2>(CartErrors.CartNotFoundOrEmpty);


                var totalAmount = cart.CartProducts.Sum(cp => cp.Quantity * cp.Product.Price);

                // Create Stripe payment intent
                var stripeAmount = Convert.ToInt64(totalAmount * 100); // Convert to piasters (EGP subunits)

                var paymentIntentService = new PaymentIntentService();
                var paymentIntent = await paymentIntentService.CreateAsync(new PaymentIntentCreateOptions
                {
                    Amount = stripeAmount,
                    Currency = "egp",
                    PaymentMethod = request.PaymentMethodId,
                    Confirm = true,
                    AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                    {
                        Enabled = true,
                        AllowRedirects = "never"
                    }
                }, cancellationToken: cancellationToken);

                var paymentMethodService = new PaymentMethodService();
                var paymentMethod = await paymentMethodService.GetAsync(
                    paymentIntent.PaymentMethodId,  // Get ID from PaymentIntent
                    cancellationToken: cancellationToken
                );

                // Format payment method description
                var paymentMethodDescription = paymentMethod?.Card != null
                    ? $"{paymentMethod.Card.Brand}"
                    : "Card payment";

                if (paymentIntent.Status != "succeeded")
                {
                    return Result.Failure<OrderResponseV2>(OrderErrors.PaymentProcessingFailed);
                }


                var order = new Order
                {
                    ApplicationUserId = userId,
                    TotalAmount = totalAmount,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending
                };

                _context.Orders!.Add(order);
                await _context.SaveChangesAsync(cancellationToken);

                // Create shipping records per vendor
                var vendorGroups = cart.CartProducts
                    .GroupBy(cp => cp.Product.VendorId)
                    .ToList();

                foreach (var vendorGroup in vendorGroups)
                {
                    _context.Shippings!.Add(new Shipping
                    {
                        OrderId = order.Id,
                        VendorId = vendorGroup.Key,
                        Status = ShippingStatus.Pending,
                        EstimatedDeliveryDate = DateTime.UtcNow.AddDays(3)
                    });
                }

                // Update order status based on initial shipping statuses
                UpdateOrderStatus(order);

                var payment = new Payment
                {
                    OrderId = order.Id,
                    TotalAmount = order.TotalAmount,
                    Status = paymentIntent.Status == "succeeded"
                    ? PaymentStatus.Completed
                    : PaymentStatus.Failed,
                    PaymentMethod = paymentMethodDescription,
                    TransactionDate = DateTime.UtcNow
                };

                _context.Payments!.Add(payment);

                var orderProducts = cart.CartProducts.Select(cp => new OrderProduct
                {
                    OrderId = order.Id,
                    ProductId = cp.ProductId,
                    Quantity = cp.Quantity
                }).ToList();

                _context.OrderProducts!.AddRange(orderProducts);
                _context.CartProducts!.RemoveRange(cart.CartProducts);

                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return await GetOrderAsync(order.Id);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result.Failure<OrderResponseV2>(OrderErrors.OrderCreationFaild);
            }
        }

        public async Task<Result<OrderResponseV2>> AddOrderWithCashPaymentAsync(string userId,  CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var cart = await _context.Carts!
                    .Include(c => c.CartProducts)
                    .ThenInclude(cp => cp.Product)
                    .FirstOrDefaultAsync(c => c.ApplicationUserId == userId, cancellationToken: cancellationToken);


                if (cart == null || !cart.CartProducts.Any())
                    return Result.Failure<OrderResponseV2>(CartErrors.CartNotFoundOrEmpty);


                var totalAmount = cart.CartProducts.Sum(cp => cp.Quantity * cp.Product.Price);                

                var order = new Order
                {
                    ApplicationUserId = userId,
                    TotalAmount = totalAmount,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending
                };

                _context.Orders!.Add(order);
                await _context.SaveChangesAsync(cancellationToken);

                // Create shipping records per vendor
                var vendorGroups = cart.CartProducts
                    .GroupBy(cp => cp.Product.VendorId)
                    .ToList();

                foreach (var vendorGroup in vendorGroups)
                {
                    _context.Shippings!.Add(new Shipping
                    {
                        OrderId = order.Id,
                        VendorId = vendorGroup.Key,
                        Status = ShippingStatus.Pending,
                        EstimatedDeliveryDate = DateTime.UtcNow.AddDays(3)
                    });
                }

                // Update order status based on initial shipping statuses
                UpdateOrderStatus(order);

                var payment = new Payment
                {
                    OrderId = order.Id,
                    TotalAmount = order.TotalAmount,
                    Status = PaymentStatus.Pending,
                    PaymentMethod = "CashOnDelivery",
                    TransactionDate = DateTime.UtcNow
                };

                _context.Payments!.Add(payment);

                var orderProducts = cart.CartProducts.Select(cp => new OrderProduct
                {
                    OrderId = order.Id,
                    ProductId = cp.ProductId,
                    Quantity = cp.Quantity
                }).ToList();

                _context.OrderProducts!.AddRange(orderProducts);
                _context.CartProducts!.RemoveRange(cart.CartProducts);

                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return await GetOrderAsync(order.Id);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result.Failure<OrderResponseV2>(OrderErrors.OrderCreationFaild);
            }
        }

        public async Task<Result> CancelOrderAsync(int orderId, string currentUserId, bool isAdmin, CancellationToken cancellationToken = default)
        {
            var order = await _context.Orders!
                .Include(o => o.Shippings)
                .Include(o => o.OrderProducts)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

            if (order == null)
                return Result.Failure(OrderErrors.OrderNotFound);

            // Check permissions
            if (!isAdmin && order.ApplicationUserId != currentUserId)
            {
                return Result.Failure(
                    new Error("Order.AccessDenied",
                    "You don't have permission to cancel this order",
                    StatusCodes.Status403Forbidden));
            }

            // Check if order is cancellable
            if (order.Status == OrderStatus.Delivered )
            {
                return Result.Failure(
                    new Error("Order.Completed",
                    $"Cannot cancel order in {order.Status} state",
                    StatusCodes.Status400BadRequest));
            }
            else if (order.Status == OrderStatus.Cancelled)
            {
                return Result.Failure(
                    new Error("Order.Cancelled",
                    $"Order is already cancelled",
                    StatusCodes.Status400BadRequest));
            }

            // Update order status
            order.Status = OrderStatus.Cancelled;

            // Cancel all shippings
            foreach (var shipping in order.Shippings!)
            {
                shipping.Status = ShippingStatus.Cancelled;
            }

            // Handle payment reversal
            if (order.Payment.Status == PaymentStatus.Completed)
            {
                // Initiate refund process
                order.Payment.Status = PaymentStatus.Refunded;
                // BackgroundService would handle actual refund
            }
            else
            {
                order.Payment.Status = PaymentStatus.Cancelled;
            }

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result<OrderResponseV2>> UpdateShipmentStatusAsync(int orderId, string vendorId, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default)
        {
            var shipping = await _context.Shippings!
               .Include(s => s.Order)
                   .ThenInclude(o => o.Shippings)
               .Include(s => s.Vendor)
               .FirstOrDefaultAsync(s =>
               s.OrderId == orderId &&
               s.VendorId == vendorId,
               cancellationToken);

            if (shipping == null)
                return Result.Failure<OrderResponseV2>(OrderErrors.ShippingNotFound);

            // Permission check
            if (shipping.VendorId != vendorId)
            {
                return Result.Failure<OrderResponseV2>(
                    new Error("Shipping.AccessDenied",
                    "You don't have permission to update this shipping",
                StatusCodes.Status403Forbidden));
            }

            // Parse and update status
            var newStatus = (ShippingStatus)Enum.Parse(typeof(ShippingStatus), request.NewStatus, true);
            shipping.Status = newStatus;

            UpdateOrderStatus(shipping.Order);

            await _context.SaveChangesAsync(cancellationToken);

            var bodyMessage = newStatus switch
            {
                ShippingStatus.Preparing =>
                    "طلبك قيد التحضير حاليًا. سنقوم بإبلاغك فور خروجه للتوصيل.",

                ShippingStatus.OutForDelivery =>
                    "طلبك في طريقه إليك الآن. يُرجى التأكد من توفرك للاستلام.",

                ShippingStatus.Delivered =>
                    "تم تسليم طلبك بنجاح. نأمل أن تكون تجربتك مرضية.",

                _ => $"تم تحديث حالة طلبك إلى: {newStatus}"
            };

            var evt = new NotificationMessage
            {
                Title = "تحديث حالة الطلب",
                Body = bodyMessage,
                Type = NotificationType.UserSpecific,
                Channels = new List<ChannelType> { ChannelType.Email },
                TargetUsers = new List<string> { shipping.Order.ApplicationUserId },
                Category = NotificationCategory.Update
            };

            await _publishEndpoint.Publish(evt, ctx =>
            {
                ctx.SetRoutingKey("user.notification.created");
            });

            return await GetOrderAsync(orderId);
        }

        public async Task<Result<PaginatedList<OrdersOfVendorResponse>>> GetVendorsOrders(string vendorId,RequestFilter request,CancellationToken cancellationToken = default)
        {

            var vendorExists = await _context.Users
                .AnyAsync(u => u.Id == vendorId && u.IsApproved, cancellationToken);

            if (!vendorExists)
                return Result.Failure<PaginatedList<OrdersOfVendorResponse>>(VendorErrors.NotFound);

            // Start from Orders instead of OrderProducts
            var baseQuery = _context.Orders!
                .Where(o => o.OrderProducts.Any(op => op.Product.VendorId == vendorId))
                .OrderByDescending(o => o.OrderDate)
                .AsQueryable();

            // Apply sorting
            if (!string.IsNullOrEmpty(request.SortColumn))
            {
                baseQuery = baseQuery.OrderBy($"{request.SortColumn} {request.SortDirection}");
            }

            // Apply status filter
            if (request.Statuses != null && request.Statuses.Any())
            {
                var statusEnums = request.Statuses
                    .Select(s => Enum.Parse<OrderStatus>(s, ignoreCase: true))
                    .ToList();
                baseQuery = baseQuery.Where(o => statusEnums.Contains(o.Status));
            }

            // Apply date filter
            if (request.DateFilter != null && request.DateFilter.HasValue)
            {
                baseQuery = baseQuery.Where(o =>
                    DateOnly.FromDateTime(o.OrderDate) == request.DateFilter.Value);
            }

            // Project to response with vendor-specific products
            var query = baseQuery.Select(o => new OrdersOfVendorResponse(
                o.Id,
                o.OrderProducts
                    .Where(op => op.Product.VendorId == vendorId)
                    .Sum(op => op.Quantity * op.Product.Price),
                o.OrderDate,
                o.Status.ToString(),
                o.OrderProducts
                    .Where(op => op.Product.VendorId == vendorId)
                    .Select(op => new OrderProductResponse(
                        op.Product.Id,
                        op.Product.NameEn,
                        op.Product.NameAr,
                        op.Product.Price,
                        op.Quantity
                    ))
                    .ToList()
            ));

            var orders = await PaginatedList<OrdersOfVendorResponse>.CreateAsync(
                query.AsNoTracking(),
                request.PageNumer,
                request.PageSize,
                cancellationToken
            );

            return Result.Success(orders);
        }

        public async Task<Result<OrdersOfVendorResponse>> GetVendorOrderAsync(int orderId,string vendorId,CancellationToken cancellationToken = default)
        {
            // Fetch order with related data
            var order = await _context.Orders
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .Where(o => o.Id == orderId)
                .FirstOrDefaultAsync(cancellationToken);

            if (order == null)
                return Result.Failure<OrdersOfVendorResponse>(OrderErrors.OrderNotFound);

            // Filter products belonging to this vendor
            var vendorProducts = order.OrderProducts
                .Where(op => op.Product.VendorId == vendorId)
                .ToList();

            if (!vendorProducts.Any())
                return Result.Failure<OrdersOfVendorResponse>(OrderErrors.NoProductsForVendor);

            // Map to response DTO
            var productsResponse = vendorProducts.Select(op => new OrderProductResponse(
                op.ProductId,
                op.Product.NameEn,
                op.Product.NameAr,
                op.Product.Price,
                op.Quantity
                //op.Product.ImageUrl
            )).ToList();

            var respone =  new OrdersOfVendorResponse(
                order.Id,
                order.TotalAmount,
                order.OrderDate,
                order.Status.ToString(),
                productsResponse
            );

            return Result.Success(respone);
        }
        public async Task<Result<OrderResponse>> CheckoutAsync(CheckoutRequest request, CancellationToken cancellationToken)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var cart = await _context.Carts!
                    .Include(c => c.CartProducts)
                    .ThenInclude(cp => cp.Product)
                    .FirstOrDefaultAsync(c => c.ApplicationUserId == request.UserId, cancellationToken: cancellationToken);

                if (cart == null || !cart.CartProducts.Any())
                    return Result.Failure<OrderResponse>(CartErrors.CartNotFoundOrEmpty);

                var order = new Order
                {
                    ApplicationUserId = request.UserId,
                    TotalAmount = cart.CartProducts.Sum(cp => cp.Quantity * cp.Product.Price),
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                };

                await _context.Orders!.AddAsync(order, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                var payment = new Payment
                {
                    OrderId = order.Id,
                    TotalAmount = order.TotalAmount,
                    Status = PaymentStatus.Completed,
                    PaymentMethod = "Credit Card"
                };

               await _context.Payments!.AddAsync(payment, cancellationToken);
                _context.CartProducts!.RemoveRange(cart.CartProducts);
                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
                return Result.Success(order.Adapt<OrderResponse>());
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result.Failure<OrderResponse>(CheckOutErrors.CheckOutFaild);
            }
        }

        public async Task<Result<OrderResponseVersion3>> GetOrderDetailsAsync(int orderId , string currentUserId, bool isAdmin, CancellationToken cancellationToken = default)
        {
            var order = await _context.Orders!
                     .Include(o => o.OrderProducts)!
                         .ThenInclude(op => op.Product)!
                             .ThenInclude(p => p.Vendor)
                     .Include(o => o.Payment)
                     .Include(o => o.Shippings)
                     .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken: cancellationToken);

            if (order == null)
                return Result.Failure<OrderResponseVersion3>(OrderErrors.OrderNotFound);

            if (!isAdmin && order.ApplicationUserId != currentUserId)
            {
                return Result.Failure<OrderResponseVersion3>(new Error("Order.AccessDenied", "You don't have permission to access this order", StatusCodes.Status403Forbidden));
            }

            var vendorGroups = order.OrderProducts
                .GroupBy(op => op.Product!.Vendor!)
                .Select(group =>
                {
                    var vendor = group.Key;
                    var shipping = order.Shippings!
                        .FirstOrDefault(s => s.VendorId == vendor.Id);

                    return new VendorSummaryResponse2(
                        vendor.BusinessName ?? "Unknown Vendor",
                        group.Sum(op => op.Product!.Price * op.Quantity),
                        group.Sum(op => op.Quantity),
                        group.Select(op => new VendorOrderItemResponse(
                            op.ProductId,
                            op.Product!.MainImageUrl!,
                            op.Product.NameEn,
                            op.Product.NameAr,
                            op.Product.Price,
                            op.Quantity
                        )).ToList(),
                        shipping?.EstimatedDeliveryDate,
                        shipping?.Status.ToString(),
                        vendor.PhoneNumber ?? "Not Available"
                    );
                }).ToList();

            var response = new OrderResponseVersion3(
                order.Id,
                vendorGroups
            );

            return Result.Success(response);
        }


        public async Task<Result<VendorOrderDetailDto>> GetOrderForSpecificVendorAsync(int OrderId, string VendorId, string currentUserId, bool isAdmin, CancellationToken cancellationToken = default)
        {
            var order = await _context.Orders!
                    .Include(o => o.OrderProducts)!
                    .ThenInclude(op => op.Product)!
                    .ThenInclude(p => p.Vendor)
                    .Include(o => o.User)
                    .FirstOrDefaultAsync(o => o.Id == OrderId, cancellationToken);

            if (order == null)
                return Result.Failure<VendorOrderDetailDto>(OrderErrors.OrderNotFound);

            if (!isAdmin && order.ApplicationUserId != currentUserId)
            {
                return Result.Failure<VendorOrderDetailDto>(new Error("Order.AccessDenied", "You don't have permission to access this order", StatusCodes.Status403Forbidden));
            }

            var vendorProducts = order.OrderProducts
                .Where(op => op.Product!.VendorId == VendorId)
                .ToList();

            if (!vendorProducts.Any())
                return Result.Failure<VendorOrderDetailDto>(OrderErrors.OrderNotFound);

            var vendor = vendorProducts.First().Product!.Vendor!;
            var shipping = await _context.Shippings!
                .FirstOrDefaultAsync(s => s.OrderId == OrderId && s.VendorId == VendorId, cancellationToken);

            var dto = new VendorOrderDetailDto
            (
                new VendorOrderDto(
                    order.Id,
                    vendor.Id,
                    vendor.BusinessName!,
                    vendor.ProfilePictureUrl!,
                    order.Status.ToString(),
                    order.OrderDate,
                    vendorProducts.Sum(op => op.Quantity),
                    vendorProducts.Sum(op => op.Product!.Price * op.Quantity)
                ),

               
                vendorProducts.Select(op =>
                {
                    var userReview = op.Product!.Reviews!
                        .FirstOrDefault(r => r.ApplicationUserId == order.ApplicationUserId && r.ProductId == op.Product.Id);

                    return new VendorProductsInOrderDto
                    (
                        op.Product.Id,
                        op.Product.MainImageUrl!,
                        op.Product.NameEn,
                        op.Product.NameAr,
                        op.Product.Price,
                        op.Quantity,
                        userReview != null,
                        userReview?.Rating?? 0
                    );
                }).ToList(),

                order.User?.Address ?? "Not available",
                vendor.PhoneNumber!,
                shipping?.EstimatedDeliveryDate,
                shipping?.Status.ToString()
            );

            return Result.Success(dto);
        }


        private void UpdateOrderStatus(Order order)
        {
            // Get all shipping statuses for this order
            var shippingStatuses = order.Shippings!.Select(s => s.Status).ToList();

            if (shippingStatuses.All(s => s == ShippingStatus.Delivered))
            {
                order.Status = OrderStatus.Delivered;
            }
            else if (shippingStatuses.All(s => s == ShippingStatus.OutForDelivery))
            {
                order.Status = OrderStatus.Shipped;
            }
            else if (shippingStatuses.All(s => s == ShippingStatus.Preparing))
            {
                order.Status = OrderStatus.Processing;
            }
            else
            {
                order.Status = OrderStatus.Pending;
            }
        }
    }
    
}
