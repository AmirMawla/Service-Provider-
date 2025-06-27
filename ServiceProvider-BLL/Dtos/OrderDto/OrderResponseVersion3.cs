using ServiceProvider_BLL.Dtos.OrderProductDto;
using ServiceProvider_BLL.Dtos.PaymentDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.OrderDto;
public record OrderResponseVersion3
(
    int Id,
    List<VendorSummaryResponse2> VendorGroups
);

public record VendorSummaryResponse2(
     string BusinessName,
     decimal TotalPrice,
     int ItemCount,
     List<VendorOrderItemResponse> Items,
     DateTime? EstimatedDeliveryDate,
     string? ShipementStatus,
     string VendorPhone

 );