using ServiceProvider_BLL.Dtos.OrderProductDto;
using ServiceProvider_BLL.Dtos.PaymentDto;
using ServiceProvider_BLL.Dtos.ShippingDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.OrderDto
{
    public record OrderResponseV2(
        int Id,
        decimal TotalAmount,
        DateTime OrderDate,
        string Status,
        List<OrderProductResponse> Products,
        List<VendorSummaryResponse> VendorGroups,
        PaymentResponse Payment
        //ShippingResponse? Shipping
    );

    public record VendorSummaryResponse(
        string BusinessName,
        decimal TotalPrice,
        int ItemCount,
        List<VendorOrderItemResponse> Items
    );

    public record VendorOrderItemResponse(
        int ProductId,
        string ProductImageUrl,
        string NameEn,
        string NameAr,
        decimal Price,
        int Quantity
    );



}
