using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.VendorDto
{
    public record TopVendorResponse(
       string Id,
       string FullName,
       string? BusinessName,
       string BusinessType,
       string? ProfilePictureUrl,
       string? CoverPictureUrl,
       float? Rating,
       string Category,
       int OrderCount
    );
}
