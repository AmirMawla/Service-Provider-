﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.ProductDto
{
    public record UpdateProductRequest(
        string NameEn,
        string NameAr,
        string? Description,
        decimal Price
        );
    
}
