using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.BannersDto
{
    public record VendorBannersResponse
    (
        string Id,
        string FullName,
        string? BusinessName ,
        string BusinessType ,
        string? TaxNumber ,
        float? Rating ,
        string? ProfilePictureUrl ,
        string? CoverImageUrl 
    );
}
