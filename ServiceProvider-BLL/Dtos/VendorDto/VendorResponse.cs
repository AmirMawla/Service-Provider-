using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.VendorDto
{
    public record VendorResponse(
        string Id,
        string FullName,
        string Email,
        string BusinessName,
        string BusinessType,
        string TaxNumber,
        bool IsApproved
    );

}
