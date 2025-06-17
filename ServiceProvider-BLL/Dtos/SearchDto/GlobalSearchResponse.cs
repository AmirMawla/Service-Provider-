using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.SearchDto
{
    public record GlobalSearchResponse(
      string? Type = null, // "Vendor", "Product", "Category", "SubCategory"
      string? Id = null,
      string? NameEn = null,
      string? NameAr = null,
      string? FullName = null,
      string? BusinessName = null,
      string? BusinessType = null,
      string? CategoryNameEn = null,
      string? CategoryNameAr = null,
      int? CategoryId = null,
      string? ImageUrl = null
    );
}
