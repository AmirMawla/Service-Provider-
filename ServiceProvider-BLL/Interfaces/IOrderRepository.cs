using SeeviceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.Common;
using ServiceProvider_BLL.Dtos.OrderDto;
using ServiceProvider_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Interfaces
{
    public interface IOrderRepository : IBaseRepository<Order>
    {
        Task<Result<OrderResponseV2>> GetOrderAsync(int orderId, string currentUserId, bool isAdmin, CancellationToken cancellationToken = default);
        Task<Result<VendorOrderDetailDto>> GetOrderForSpecificVendorAsync(int orderId,string VendorId, string currentUserId, bool isAdmin ,  CancellationToken cancellationToken = default);
        Task<Result<PaginatedList<OrderResponse>>> GetAllOrderAsync(RequestFilter request, CancellationToken cancellationToken = default);
        Task<Result<PaginatedList<VendorOrderDto>>> GetUserOrdersAsync(string userId, RequestFilter request, CancellationToken cancellationToken = default);
        Task<Result<PaginatedList<OrdersOfVendorResponse>>> GetVendorsOrders(string vendorId, RequestFilter request, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<RecentOrderResponse>>> GetTopFiveRecentOrdersAsync(string vendorId, int count = 5, CancellationToken cancellationToken = default);
        Task<Result<OrderResponseV2>> AddOrderAsync(string userId, OrderRequest request, CancellationToken cancellationToken = default);
        Task<Result<OrderResponseV2>> UpdateOrderStatusAsync(int orderId, string currentUserId, bool isAdmin, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default);
        Task<Result<OrderResponse>> CheckoutAsync(CheckoutRequest request , CancellationToken cancellationToken);


        
    }
}
