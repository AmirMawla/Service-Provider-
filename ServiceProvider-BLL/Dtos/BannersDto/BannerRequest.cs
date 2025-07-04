using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.BannersDto;
public record BannerRequest(
    int ProductId,
    string? Description,
    string? ImageUrl,
    decimal DiscountPercentage,
    string? DiscountCode
);
