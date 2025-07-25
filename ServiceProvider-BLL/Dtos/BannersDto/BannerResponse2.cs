﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.BannersDto;
public record BannerResponse2
(
    int ProductId,
    string? ProductNameEn,
    string? ProductNameAr,
    string? Description,
    string? ImageUrl,
    decimal DiscountPercentage,
    string? DiscountCode
);
