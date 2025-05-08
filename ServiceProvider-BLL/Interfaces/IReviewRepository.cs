using SeeviceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.Common;
using ServiceProvider_BLL.Dtos.ReviewDto;
using ServiceProvider_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Interfaces
{
    public interface IReviewRepository : IBaseRepository<Review>
    {
        Task<Result<PaginatedList<VendorReviewsResponse>>> GetRatingsByVendorAsync(string vendorId, RequestFilter request, CancellationToken cancellationToken = default);
        Task<Result<PaginatedList<ReviewResponse>>> GetAllRatingsFromAllUsersToAllVendorAsync(RequestFilter request, CancellationToken cancellationToken = default);
        Task<Result> UpdateReviewAsync(int reviewId,string userId ,UpdateReviewRequest request, CancellationToken cancellationToken = default);
        Task<Result> DeleteReviewAsync(int reviewId, string userId, CancellationToken cancellationToken = default);
    }
}
