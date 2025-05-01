using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.OrderDto
{
    public record RecentOrderResponse(
        int OrderId,
        DateTime OrderDate,
        decimal TotalAmount,
        string Status,
        string CustomerName,
        List<OrderItemResponse> Items
    );

    public record OrderItemResponse(
        string ProductNameEn,
        string ProductNameAr,
        int Quantity,
        decimal UnitPrice
    );
}
