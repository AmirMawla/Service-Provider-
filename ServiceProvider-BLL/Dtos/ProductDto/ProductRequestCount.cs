using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.ProductDto
{
    public record ProductRequestCount(
         int Id,
         string NameEn,
         string Description,
         string MainImageUrl,
         decimal Price,
         int RequestCount
    );
}
