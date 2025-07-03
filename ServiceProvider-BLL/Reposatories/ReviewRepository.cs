using Azure.Core;
using Mapster;
using Microsoft.EntityFrameworkCore;
using SeeviceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.Common;
using ServiceProvider_BLL.Dtos.ReviewDto;
using ServiceProvider_BLL.Errors;
using ServiceProvider_BLL.Interfaces;
using ServiceProvider_DAL.Data;
using ServiceProvider_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Reposatories
{
    public class ReviewRepository : BaseRepository<Review>, IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }


        public async Task<Result<PaginatedList<VendorReviewsResponse>>> GetRatingsByVendorAsync(string vendorId, RequestFilter request , CancellationToken cancellationToken = default) 
        {
            var query = _context.Reviews!
                .Where(x => x.Product.VendorId == vendorId)
                .AsNoTracking();

            if (!query.Any())
                return Result.Failure<PaginatedList<VendorReviewsResponse>>(ReviewErrors.VendorReviewsNotFound);


            if (!string.IsNullOrEmpty(request.SearchValue))
            {
                var searchTerm = $"%{request.SearchValue.ToLower()}%";
                query = query.Where(x =>
                    EF.Functions.Like(x.User.FullName.ToLower(), searchTerm) ||
                    EF.Functions.Like(x.Product.NameEn.ToLower(), searchTerm) ||
                    EF.Functions.Like(x.Product.NameAr, searchTerm) 
                );
            }

            if (!string.IsNullOrEmpty(request.SortColumn))
            {
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");
            }

            if (request.MinRating.HasValue)
            {
                query = query.Where(x => x.Rating >= request.MinRating.Value);
            }

            if (request.MaxRating.HasValue)
            {
                query = query.Where(x => x.Rating <= request.MaxRating.Value);
            }

            var source = query.Select(x => new VendorReviewsResponse(
                x.Id,
                x.Rating,
                x.Comment,
                x.CreatedAt,
                x.User.FullName,
                x.Product.NameEn,
                x.Product.NameAr
            ));

            var reviews = await PaginatedList<VendorReviewsResponse>.CreateAsync(source, request.PageNumer, request.PageSize, cancellationToken);

            return Result.Success(reviews);
        }

        public async Task<Result<PaginatedList<ReviewResponse>>> GetAllRatingsFromAllUsersToAllVendorAsync( RequestFilter request, CancellationToken cancellationToken = default)
        {
            var query = _context.Reviews!
                .Include(x =>x.User)
                .Include(x => x.Product)
                .ThenInclude(x=>x.Vendor)
                .AsNoTracking();

            if (!query.Any())
                return Result.Failure<PaginatedList<ReviewResponse>>(ReviewErrors.ReviewsNotFound);

            if (!string.IsNullOrEmpty(request.SearchValue))
            {
                var searchTerm = $"%{request.SearchValue.ToLower()}%";
                query = query.Where(x =>
                    EF.Functions.Like(x.User.FullName.ToLower(), searchTerm) ||
                    EF.Functions.Like(x.Product.Vendor.FullName ?? "".ToLower(), searchTerm) ||
                    EF.Functions.Like(x.Product.NameEn ?? "".ToLower(), searchTerm)||
                    EF.Functions.Like(x.Product.NameAr ?? "".ToLower(), searchTerm)
                );
            }

            if (!string.IsNullOrEmpty(request.SortColumn))
            {
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");
            }

            if (request.MinRating.HasValue)
            {
                query = query.Where(x => x.Rating >= request.MinRating.Value);
            }

            if (request.MaxRating.HasValue)
            {
                query = query.Where(x => x.Rating <= request.MaxRating.Value);
            }

            var source = query.Select(r => new ReviewResponse(
                r.Id,
                r.Product.NameEn,
                r.Product.NameAr,
                r.User.FullName,
                r.User.Email,
                r.Product.Vendor.FullName,
                r.Rating,
                r.Comment,
                r.CreatedAt
                ));

            var reviews = await PaginatedList<ReviewResponse>.CreateAsync(source, request.PageNumer, request.PageSize, cancellationToken);

            return Result.Success(reviews);
        }
        public async Task<Result> UpdateReviewAsync(int reviewId,string userId ,UpdateReviewRequest request, CancellationToken cancellationToken = default)
        {
            var review = await _context.Reviews!.FindAsync(reviewId, cancellationToken);

            if(review == null)
                return Result.Failure(ReviewErrors.ReviewNotFound);

            if (review.ApplicationUserId != userId)
                return Result.Failure(ReviewErrors.Forbid);

            review.Rating = request.Rating;
            review.Comment = request.Comment;
            
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();

        }

        public async Task<Result> DeleteReviewAsync(int reviewId, string userId, CancellationToken cancellationToken = default)
        {
            var review = await _context.Reviews!.FindAsync(reviewId,cancellationToken);
           
            if (review == null)
                return Result.Failure(ReviewErrors.ReviewNotFound);

            if (review.ApplicationUserId != userId)
                return Result.Failure(ReviewErrors.Forbid);

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();

        }
        
 
    }
}
