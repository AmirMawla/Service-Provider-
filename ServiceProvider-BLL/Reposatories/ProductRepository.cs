using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SeeviceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.Common;
using ServiceProvider_BLL.Dtos.ProductDto;
using ServiceProvider_BLL.Dtos.ReviewDto;
using ServiceProvider_BLL.Dtos.VendorDto;
using ServiceProvider_BLL.Errors;
using ServiceProvider_BLL.Interfaces;
using ServiceProvider_DAL.Data;
using ServiceProvider_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ServiceProvider_BLL.Reposatories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }



        public async Task<Result<PaginatedList<ProductResponse>>> GetAllProductsAsync(RequestFilter request,CancellationToken cancellationToken = default)
        {
            var query =  _context.Products!.
                Include(p => p.Reviews).AsNoTracking();

            if (!query.Any())
                Result.Failure<PaginatedList<ProductResponse>>(ProductErrors.ProductsNotFound);

            if (!string.IsNullOrEmpty(request.SearchValue))
            {
                var searchTerm = $"%{request.SearchValue.ToLower()}%";
                query = query.Where(x =>
                    EF.Functions.Like(x.NameEn.ToLower(), searchTerm) ||
                    EF.Functions.Like(x.NameAr.ToLower(), searchTerm) ||
                    EF.Functions.Like(x.Vendor.FullName.ToLower(), searchTerm) ||
                    EF.Functions.Like(x.Vendor.BusinessName??"".ToLower(), searchTerm) ||
                    EF.Functions.Like(x.Description ?? "".ToLower(), searchTerm)
                );
            }


            if (!string.IsNullOrEmpty(request.SortColumn))
            {
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");
            }

            var source = query.Select(p => new ProductResponse(
                p.Id,
                p.NameEn,
                p.NameAr,
                p.MainImageUrl,
                p.Description,
                p.Vendor.FullName,
                p.Vendor.BusinessName!,
                p.Price,
                p.Reviews!.Any() ? p.Reviews!.Average(r => r.Rating) : 0
            ));

            var products = await PaginatedList<ProductResponse>.CreateAsync(
                source,
                request.PageNumer,
                request.PageSize,
                cancellationToken
            );

            return Result.Success(products);

        }

        public async Task<Result<ProductResponse>> GetProductAsync(int id, CancellationToken cancellationToken = default)
        {
            var productResponse = await _context.Products!
                .Where(p => p.Id == id)
                .Select(p => new ProductResponse(
                     p.Id,
                     p.NameEn,
                     p.NameAr,
                     p.MainImageUrl,
                     p.Description,
                     p.Vendor.FullName,
                     p.Vendor.BusinessName!,
                     p.Price,
                     p.Reviews!.Any() ? p.Reviews!.Average(r => r.Rating) : 0
                ))
                .FirstOrDefaultAsync(cancellationToken);

                    if (productResponse == null)
                    {
                        return Result.Failure<ProductResponse>(ProductErrors.ProductNotFound);
                    }

                 return Result.Success(productResponse);
        }

        public async Task<Result<ProductResponse>> AddProductAsync(string vendorId, ProductRequest request, CancellationToken cancellationToken = default)
        {
            var subCategoryExists = await _context.SubCategories!.AnyAsync(sc => sc.Id == request.SubCategoryId, cancellationToken: cancellationToken);

            if (!subCategoryExists)
                return Result.Failure<ProductResponse>(SubCategoryErrors.SubCategoryNotFound);

            var isRegistered = await _context.VendorSubCategories!
            .AnyAsync(vsc => vsc.VendorId == vendorId && vsc.SubCategoryId == request.SubCategoryId, cancellationToken: cancellationToken);

            if (!isRegistered)
                return Result.Failure<ProductResponse>(VendorErrors.VendorNotRegisterdInSubCategory);

            var product = request.Adapt<Product>();

            product.VendorId = vendorId;
            product.CreatedAt = DateTime.UtcNow;

            await _context.Products!.AddAsync(product, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(product.Adapt<ProductResponse>());

        }

        public async Task<Result> UpdateProductAsync(int id, UpdateProductRequest request, string vendorId, CancellationToken cancellationToken = default)
        {
            var currentProduct = await _context.Products!.FirstOrDefaultAsync(p => p.Id == id && p.VendorId == vendorId, cancellationToken: cancellationToken);

            if (currentProduct is null)
                return Result.Failure(ProductErrors.ProductNotFound);

            // request.Adapt(currentProduct);
            currentProduct.NameEn = request.NameEn;
            currentProduct.NameAr = request.NameAr;
            currentProduct.Description = request.Description;
            currentProduct.Price = request.Price;
            currentProduct.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result> DeleteProductAsync(int id, string vendorId, CancellationToken cancellationToken = default)
        {
            var product = await _context.Products!.FirstOrDefaultAsync(p => p.Id == id && p.VendorId == vendorId, cancellationToken: cancellationToken);

            if (product is null)
                return Result.Failure(ProductErrors.ProductNotFound);

            _context.Remove(product);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result<IEnumerable<ReviewResponse>>> GetReviewsForSpecificServiceAsync(int productId, CancellationToken cancellationToken = default)
        {
            var productExisit = await _context.Products!.AnyAsync(x => x.Id == productId, cancellationToken);

            if (!productExisit)
                return Result.Failure<IEnumerable<ReviewResponse>>(ProductErrors.ProductNotFound);

            var reviews = await _context.Reviews!
                            .Where(x => x.ProductId == productId)
                            .Include(x => x.User)
                            .Include(x => x.Product)
                            .ThenInclude(x => x.Vendor)
                            .AsNoTracking()
                            .ToListAsync(cancellationToken);

            if (!reviews.Any())
                return Result.Failure<IEnumerable<ReviewResponse>>(ReviewErrors.ReviewsNotFound);

            var response = reviews.Adapt<IEnumerable<ReviewResponse>>();

            return Result.Success(response);

        }

        public async Task<Result<ReviewResponse>> AddReviewAsync(int productId, ReviewRequest request, CancellationToken cancellationToken = default)
        {
            var productExisit = await _context.Products!.AnyAsync(x => x.Id == productId, cancellationToken);

            if (!productExisit)
                return Result.Failure<ReviewResponse>(ProductErrors.ProductNotFound);

            var hasOrderdProduct = await _context.OrderProducts!
                                    .AnyAsync(x =>
                                      x.ProductId == productId &&
                                      x.Order.ApplicationUserId == request.UserId &&
                                      x.Order.Status == OrderStatus.Delivered
                                      , cancellationToken
                                    );

            if (!hasOrderdProduct)
                return Result.Failure<ReviewResponse>(ReviewErrors.HasNotOrdered);


            var existingReview = await _context.Reviews!
                    .FirstOrDefaultAsync(r =>
                        r.ProductId == productId &&
                        r.ApplicationUserId == request.UserId, cancellationToken: cancellationToken
                    );

            if (existingReview != null)
                return Result.Failure<ReviewResponse>(ReviewErrors.DuplicatedReview);


            var review = new Review
            {
                ProductId = productId,
                ApplicationUserId = request.UserId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Reviews!.AddAsync(review, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await UpdateProductAverageRating(productId);

            var response = review.Adapt<ReviewResponse>();
            

            return Result.Success(response);
        }


        public async Task<Result<List<ProductRequestCount>>> GetMostCommonProductAsync(CancellationToken cancellationToken = default)
        {
            var query = _context.OrderProducts!
               .GroupBy(op => op.ProductId)
               .Select(g => new
               {
                   ProductId = g.Key,
                   OrderCount = g.Sum(op => op.Quantity)
               })
               .OrderByDescending(x => x.OrderCount);

            var productIds = await query.Select(x => x.ProductId).ToListAsync(cancellationToken: cancellationToken);

            var productsWithDetails = await _context.Products!
                .Where(p => productIds.Contains(p.Id))
                .Select(p => new
                {
                    p.Id,
                    p.NameEn,
                    p.Description,
                    p.MainImageUrl,
                    p.Price,
                    RequestCount = _context.OrderProducts!
                            .Where(op => op.ProductId == p.Id)
                            .Sum(op => op.Quantity)
                }).OrderByDescending(p => p.RequestCount).ToListAsync(cancellationToken: cancellationToken);


            return Result.Success(productsWithDetails.Adapt<List<ProductRequestCount>>());


        }

        public async Task<Result<IEnumerable<ProductResponse>>> GetNewProductsAsync(CancellationToken cancellationToken = default) 
        {
            var products = await _context.Products!
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync(cancellationToken: cancellationToken);

            if (!products.Any()) 
                return Result.Failure<IEnumerable<ProductResponse>>(ProductErrors.ProductsNotFound);

            return Result.Success(products.Adapt<IEnumerable<ProductResponse>>());
        }


        private async Task UpdateProductAverageRating(int productId)
        {
            var productReviews = await _context.Reviews!
                .Where(r => r.ProductId == productId)
                .ToListAsync();

            if (productReviews.Any())
            {
                var averageRating = productReviews.Average(r => r.Rating);

                var product = await _context.Products!.FindAsync(productId);
                if (product != null)
                {
                    var vendor = await _context.Users.FindAsync(product.VendorId);
                    if (vendor != null)
                    {
                        // Update vendor's rating based on all their product reviews
                        var vendorProducts = await _context.Products
                            .Where(p => p.VendorId == product.VendorId)
                            .ToListAsync();

                        var vendorProductReviews = await _context.Reviews!
                            .Where(r => vendorProducts.Select(p => p.Id).Contains(r.ProductId))
                            .ToListAsync();

                        if (vendorProductReviews.Any())
                        {
                            vendor.Rating = vendorProductReviews.Average(r => (float)r.Rating);
                        }

                        _context.Users.Update(vendor);
                    }

                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
