﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.ProductDto
{
    public record ProductResponse(
        int Id,
        string NameEn,
        string NameAr,
        string ImageUrl,
        string? Description,
        decimal Price,
        double Rating
    );
    
}
