using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.OrderDto
{
    public record OrderResponse(
        int Id,
        string ApplicationUserId,
        string ApplicationUserFullName,
        List<VendorOrderResponse> Vendors,
        decimal TotalAmount,
        DateTime OrderDate,
        string Status
    );

    public record VendorOrderResponse(
      string Id,
      string FullName,
      string BusinessName,
      string BusinessType
    );
    
}
