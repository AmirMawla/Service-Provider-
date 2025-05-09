using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.ProductDto;
public record ProductDto(
        int Id,
        string NameEn,
        string NameAr,
        string? MainImageUrl,
        string? Description,
        decimal Price
    );
