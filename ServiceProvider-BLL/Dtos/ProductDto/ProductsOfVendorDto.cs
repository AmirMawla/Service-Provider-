using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.ProductDto
{
    public record ProductsOfVendorDto(
        int Id,
        string NameEn,
        string NameAr,
        string Description,
        string MainImageUrl,
        decimal Price,
        string CategoryNameEn,
        string CategoryNameAr
        );
}
