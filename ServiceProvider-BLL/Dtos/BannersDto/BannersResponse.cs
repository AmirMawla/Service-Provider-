using ServiceProvider_BLL.Dtos.VendorDto;
using ServiceProvider_BLL.Dtos.ProductDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.BannersDto
{
    public record BannersResponse
    (
     int Id,
     string? Description,
     string ImageUrl,
     decimal? DiscountPercentage,
     VendorBannersResponse?  Vendor ,
     ProductBannersResponse? Product

     );

    
}
