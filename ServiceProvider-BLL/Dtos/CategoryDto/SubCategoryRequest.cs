using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.CategoryDto
{
    public record SubCategoryRequest(
        string NameEn,
        string NameAr,
        string? ImageUrl

    );

}
