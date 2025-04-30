using Mapster;
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
using System;
using System.Collections.Generic;
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

        public async Task<Result<OrderResponseV2>> GetOrderAsync(int orderId, CancellationToken cancellationToken = default)
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

        public async Task<Result<IEnumerable<OrderResponseV2>>> GetUserOrdersAsync(string userId , CancellationToken cancellationToken = default)
        {
            var orders = await _context.Orders!
                .Where(o => o.ApplicationUserId == userId)
                .Include(o => o.OrderProducts)
                .Include(o => o.Payment)
                .Select(o => new OrderResponseV2(
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
                    new PaymentResponse(
                        o.Payment.TotalAmount,
                        o.Payment.Status.ToString(),
                        o.Payment.TransactionDate
                    ),
                    o.Shipping != null ? new ShippingResponse(
                        o.Shipping.Status,
                        o.Shipping.EstimatedDeliveryDate
                    ) : null
                ))
                .ToListAsync(cancellationToken: cancellationToken);

            return Result.Success(orders.Adapt<IEnumerable<OrderResponseV2>>());
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

                var order = new Order
                {
                    ApplicationUserId = userId,
                    TotalAmount = cart.CartProducts.Sum(cp => cp.Quantity * cp.Product.Price),
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending
                };

                _context.Orders!.Add(order);
                await _context.SaveChangesAsync(cancellationToken);

                var payment = new Payment
                {
                    OrderId = order.Id,
                    TotalAmount = order.TotalAmount,
                    Status = PaymentStatus.Pending,
                    PaymentMethod = request.PaymentMethod,
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

        public async Task<Result<OrderResponseV2>> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default)
        {
            var order = await _context.Orders!.FindAsync(orderId , cancellationToken);
            if (order == null)
                return Result.Failure<OrderResponseV2>(OrderErrors.OrderNotFound);

            order.Status = (OrderStatus)Enum.Parse(typeof(OrderStatus), request.NewStatus, true); 
            await _context.SaveChangesAsync(cancellationToken);

            return await GetOrderAsync(orderId);
        }

        public async Task<Result<IEnumerable<OrdersOfVendorResponse>>> GetVendorsOrders(string vendorId,CancellationToken cancellationToken = default) 
        {
            var orders = await _context.Orders!
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .Where(o => o.OrderProducts.Any(op => op.Product.VendorId == vendorId))
                .Select(x => new OrdersOfVendorResponse(
                    x.Id,
                    x.TotalAmount,
                    x.OrderDate,
                    x.Status.ToString(),
                    x.OrderProducts.Select(op => new OrderProductResponse(
                        op.ProductId,
                        op.Product.NameEn,
                        op.Product.NameAr,
                        op.Product.Price,
                        op.Quantity
                    )).ToList()
                )).ToListAsync(cancellationToken);

            if (!orders.Any())
            {
                return Result.Failure<IEnumerable<OrdersOfVendorResponse>>(OrderErrors.NoOrdersForThisVendor);
            }

            return Result.Success<IEnumerable<OrdersOfVendorResponse>>(orders);
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


    }
}
