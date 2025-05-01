using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.AnalyticsDto.cs
{
    public record VendorRevenueResponse(
      string VendorId,
      string FullName,
      string? BusinessName,
      string BusinessType,
      string? ProfilePictureUrl,
      string? CoverImageUrl,
      decimal TotalRevenue,
      int TotalOrderCount
    );
}
