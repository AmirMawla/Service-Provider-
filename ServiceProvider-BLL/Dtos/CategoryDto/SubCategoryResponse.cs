using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.CategoryDto
{
    public record SubCategoryResponse(
        int Id ,
        string NameEn,
        string NameAr,
        string? ImageUrl

        );
}
