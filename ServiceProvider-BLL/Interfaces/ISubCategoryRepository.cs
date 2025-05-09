using SeeviceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.CategoryDto;
using ServiceProvider_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Interfaces
{
    public interface ISubCategoryRepository : IBaseRepository<SubCategory>
    {
        Task<Result<IEnumerable<SubCategoryResponse>>> GetSubCategoriesAsync(CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<SubCategoryResponse>>> GetSubCategoriesUnderVendorAsync(string ProviderId ,CancellationToken cancellationToken = default);
    }
}
