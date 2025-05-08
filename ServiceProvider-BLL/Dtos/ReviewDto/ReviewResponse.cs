using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.ReviewDto
{
    public record ReviewResponse(
        int Id,
        string ProductNameEn,
        string ProductNameAr,
        string UserFullName,
        string UserEmail,
        string VendorFullName,
        int Rating,
        string? Comment,
        DateTime CreatedAt
        );
}
