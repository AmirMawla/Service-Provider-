using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.OrderDto;
public record VendorOrderDto
(
    int OrderId,
    string VendorId,
    string VendorName ,
   string VendorImageUrl ,
   string OrderStatus ,
    DateTime OrderDate ,
   int TotalItems,
   decimal TotalAmount 
);