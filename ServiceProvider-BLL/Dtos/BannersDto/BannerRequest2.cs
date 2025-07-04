using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.BannersDto;
public record BannerRequest2(
    int ProductId,
    string? Description,
    IFormFile? ImageUrl,
    decimal DiscountPercentage,
    string? DiscountCode
);