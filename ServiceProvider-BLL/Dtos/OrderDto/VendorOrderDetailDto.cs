using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.OrderDto;
public record VendorOrderDetailDto
    (
    VendorOrderDto VendorOrderDto,
    List<VendorOrderItemResponse> VendorOrderItemResponse,
    string UserAddress ,
    string VendorPhone ,
    DateTime? EstimatedDeliveryDate

    );
