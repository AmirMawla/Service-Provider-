using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.ProductDto;
public record CreateProductDto
(
       string NameEn,
       string NameAr,
       string? Description,
       decimal Price,
       IFormFile? Image ,
       int SubCategoryId 
    
);
