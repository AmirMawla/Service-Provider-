using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.ProductDto
{
    public record MostRequestedProductResponse(
       int ProductId,
       string ProductNameEn,
       string ProductNameAr,
       string ImageUrl,
       int TotalOrders,
       decimal TotalRevenue
    );

}
