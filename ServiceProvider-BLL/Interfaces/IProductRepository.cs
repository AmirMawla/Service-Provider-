using SeeviceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.Common;
using ServiceProvider_BLL.Dtos.ProductDto;
using ServiceProvider_BLL.Dtos.ReviewDto;
using ServiceProvider_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Interfaces
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<Result<PaginatedList<ProductResponse>>> GetAllProductsAsync(RequestFilter request, CancellationToken cancellationToken = default);
        Task<Result<ProductResponse>> GetProductAsync(int id , CancellationToken cancellationToken = default);
        Task<Result<List<ProductRequestCount>>> GetMostCommonProductAsync(CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<ProductResponse>>> GetNewProductsAsync(CancellationToken cancellationToken = default);
        Task<Result<ProductResponse>> AddProductAsync(string vendorId,ProductRequest request, CancellationToken cancellationToken = default);
        Task<Result> UpdateProductAsync(int id, UpdateProductRequest request, string vendorId, CancellationToken cancellationToken = default);
        Task<Result> DeleteProductAsync(int id, string vendorId, CancellationToken cancellationToken = default);

        Task<Result<ReviewResponse>> AddReviewAsync(int productId, ReviewRequest request, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<ReviewResponse>>> GetReviewsForSpecificServiceAsync(int productId, CancellationToken cancellationToken = default);
    }
}
