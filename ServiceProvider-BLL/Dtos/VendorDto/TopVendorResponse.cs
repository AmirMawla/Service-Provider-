using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.VendorDto
{
    public record TopVendorResponse(
       string FullName,
       string BusinessName,
       string BusinessType,
       string ProfilePictureUrl,
       string Category,
       int OrderCount
    );
}
