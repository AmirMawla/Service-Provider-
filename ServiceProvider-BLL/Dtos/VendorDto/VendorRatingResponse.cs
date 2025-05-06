using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.VendorDto
{
    public record VendorRatingResponse(
        int ReviewId,
        int Rating,
        string Comment,
        DateTime CreatedAt,
        string ProductNameEn,
        string ProductNameAr,
        string CustomerName,
        string CustomerId
    );
}
