using Azure.Core;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

namespace ServiceProvider_BLL.Reposatories
{
    public class OrderRepository : BaseRepository<Order> , IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Result<OrderResponseV2>> GetOrderAsync(int orderId,string currentUserId,bool isAdmin, CancellationToken cancellationToken = default)
        {
            var order = await _context.Orders!
                     .Include(o => o.OrderProducts)
                     .ThenInclude(op => op.Product)
                     .Include(o => o.Payment)
                     .Include(o => o.Shipping)
                     .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken: cancellationToken);

            if (order == null)
                return Result.Failure<OrderResponseV2>(OrderErrors.OrderNotFound);

            if (!isAdmin && order.ApplicationUserId != currentUserId)
            {
                return Result.Failure<OrderResponseV2>(new Error("Order.AccessDenied", "You don't have permission to access this order",StatusCodes.Status403Forbidden));
            }

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
                 ),
                 order.Shipping != null ? new ShippingResponse(
                     order.Shipping.Status,
                     order.Shipping.EstimatedDeliveryDate
                 ) : null
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

        public async Task<Result<PaginatedList<OrderResponseV2>>> GetUserOrdersAsync(string userId,RequestFilter request , CancellationToken cancellationToken = default)
        {
            var query = _context.Orders!
                .Where(o => o.ApplicationUserId == userId)
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .ThenInclude(p => p.Vendor)
                .Include(o => o.Payment)
                .Include(o => o.Shipping)
                .OrderByDescending(o => o.OrderDate)
                .AsNoTracking();

            // Apply sorting if specified
            if (!string.IsNullOrEmpty(request.SortColumn))
            {
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");
            }

            var projectedQuery = query.Select(o => new OrderResponseV2(
                o.Id,
                o.TotalAmount,
                o.OrderDate,
                o.Status.ToString(),
                o.OrderProducts.Select(op => new OrderProductResponse(
                    op.ProductId,
                    op.Product.NameEn,
                    op.Product.NameAr,
                    op.Product.Price,
                    op.Quantity
                )).ToList(),
                o.OrderProducts
                  .GroupBy(op => op.Product.Vendor.BusinessName)
                  .Select(g => new VendorSummaryResponse(
                       g.Key ?? "Unknown Vendor",
                       g.Sum(op => op.Product.Price * op.Quantity),
                       g.Sum(op => op.Quantity),
                       g.Select(op => new VendorOrderItemResponse(
                           op.ProductId,
                           op.Product.NameEn,
                           op.Product.NameAr,
                           op.Product.Price,
                           op.Quantity
                       )).ToList()
                  )).ToList(),
                new PaymentResponse(
                    o.Payment.TotalAmount,
                    o.Payment.Status.ToString(),
                    o.Payment.TransactionDate
                    //o.Payment.PaymentMethodType
                ),
                o.Shipping != null ? new ShippingResponse(
                    o.Shipping.Status,
                    o.Shipping.EstimatedDeliveryDate
                ) : null
            ));

            var orders = await PaginatedList<OrderResponseV2>.CreateAsync(
            projectedQuery,
                request.PageNumer,
                request.PageSize,
                cancellationToken
            );

            return orders.Items.Any()
                ? Result.Success(orders)
                : Result.Failure<PaginatedList<OrderResponseV2>>(OrderErrors.OrderNotFound);
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
                    ? $"{paymentMethod.Card.Brand} ending in {paymentMethod.Card.Last4}"
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

                return await GetOrderDetailsAsync(order.Id);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                return Result.Failure<OrderResponseV2>(OrderErrors.OrderCreationFaild);
            }
        }

        public async Task<Result<OrderResponseV2>> UpdateOrderStatusAsync(int orderId, string currentUserId, bool isAdmin, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default)
        {
            var order = await _context.Orders!
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .ThenInclude(p => p.Vendor)
                .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

            if (order == null)
                return Result.Failure<OrderResponseV2>(OrderErrors.OrderNotFound);

            if (!isAdmin)
            {
                // Check if current user is a vendor associated with the order
                var isOrderVendor = order.OrderProducts
                    .Any(op => op.Product.VendorId == currentUserId);

                if (!isOrderVendor)
                {
                    return Result.Failure<OrderResponseV2>(new Error("Order.AccessDenied", "You don't have permission to access this order", StatusCodes.Status403Forbidden));
                }
            }

            order.Status = (OrderStatus)Enum.Parse(typeof(OrderStatus), request.NewStatus, true); 

            await _context.SaveChangesAsync(cancellationToken);

            return await GetOrderDetailsAsync(orderId);
        }

        public async Task<Result<PaginatedList<OrdersOfVendorResponse>>> GetVendorsOrders(string vendorId,RequestFilter request,CancellationToken cancellationToken = default)
        {

            var vendorExists = await _context.Users.AnyAsync(u => u.Id == vendorId && u.IsApproved, cancellationToken: cancellationToken);
            if (!vendorExists)
            {
                return Result.Failure<PaginatedList<OrdersOfVendorResponse>>(VendorErrors.NotFound);
            }

            var baseQuery = _context.Orders!
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .Where(o => o.OrderProducts.Any(op => op.Product.VendorId == vendorId))
                .OrderByDescending(o => o.OrderDate);

          
            if (!string.IsNullOrEmpty(request.SortColumn))
            {
                baseQuery = baseQuery.OrderBy($"{request.SortColumn} {request.SortDirection}");
            }

            var query = baseQuery.Select(o => new OrdersOfVendorResponse(
                o.Id,
                o.TotalAmount,
                o.OrderDate,
                o.Status.ToString(),
                o.OrderProducts.Select(op => new OrderProductResponse(
                    op.ProductId,
                    op.Product.NameEn,
                    op.Product.NameAr,
                    op.Product.Price,
                    op.Quantity
                )).ToList()
            ));

            var orders = await PaginatedList<OrdersOfVendorResponse>.CreateAsync(
                query.AsNoTracking(),
                request.PageNumer,
                request.PageSize,
                cancellationToken
            );

            return Result.Success(orders);
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

        private async Task<Result<OrderResponseV2>> GetOrderDetailsAsync(int orderId,CancellationToken cancellationToken = default)
        {
            var order = await _context.Orders!
                     .Include(o => o.OrderProducts)
                     .ThenInclude(op => op.Product)
                     .Include(o => o.Payment)
                     .Include(o => o.Shipping)
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
                       g.Select(op =>new VendorOrderItemResponse(
                           op.ProductId,
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
                 ),
                 order.Shipping != null ? new ShippingResponse(
                     order.Shipping.Status,
                     order.Shipping.EstimatedDeliveryDate
                 ) : null
            );

            return Result.Success(response);
        }

    }
}
