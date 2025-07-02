using Mapster;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IWebHostEnvironment _env;
        public ProductRepository(AppDbContext context, IWebHostEnvironment env) : base(context)
        {
            _context = context;
            _env = env;
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
                p.Reviews!.Any() ?p.Reviews!.Average(r => r.Rating) : 0
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
                     p.Reviews!.Any() ? (double) p.Reviews!.Average(r => r.Rating) : 0.0
                ))
                .FirstOrDefaultAsync(cancellationToken);

                    if (productResponse == null)
                    {
                        return Result.Failure<ProductResponse>(ProductErrors.ProductNotFound);
                    }

                 return Result.Success(productResponse);
        }

        public async Task<Result<ProductResponse>> AddProductAsync(string vendorId, CreateProductDto request, CancellationToken cancellationToken = default)
        {
            var subCategoryExists = await _context.SubCategories!.AnyAsync(sc => sc.Id == request.SubCategoryId, cancellationToken: cancellationToken);

            if (!subCategoryExists)
                return Result.Failure<ProductResponse>(SubCategoryErrors.SubCategoryNotFound);

            var isRegistered = await _context.VendorSubCategories!
            .AnyAsync(vsc => vsc.VendorId == vendorId && vsc.SubCategoryId == request.SubCategoryId, cancellationToken: cancellationToken);

            if (!isRegistered)
                return Result.Failure<ProductResponse>(VendorErrors.VendorNotRegisterdInSubCategory);



            string? imagePath = null;

            if (request.Image != null)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "images/products");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{request.Image.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await request.Image.CopyToAsync(stream);

                imagePath = $"/images/products/{uniqueFileName}";
            }

            var product = new Product
            {
                NameEn = request.NameEn,
                NameAr = request.NameAr,
                Description = request.Description,
                Price = request.Price,
                MainImageUrl = imagePath,
                CreatedAt = DateTime.UtcNow,
                SubCategoryId = request.SubCategoryId,
                VendorId = vendorId
            };

            await _context.Products!.AddAsync(product, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(product.Adapt<ProductResponse>());

        }

        public async Task<Result<PaginatedList<ProductResponse>>> GetAllProductsUnderSubcategoryAsync(int subCategoryId,RequestFilter request, CancellationToken cancellationToken = default)
        {
            var query = _context.Products!.
                Include(p => p.Reviews)
                .Where(p => p.SubCategoryId == subCategoryId)
                .AsNoTracking();

            if (!query.Any())
                Result.Failure<PaginatedList<ProductResponse>>(ProductErrors.ProductsNotFound);

            if (!string.IsNullOrEmpty(request.SearchValue))
            {
                var searchTerm = $"%{request.SearchValue.ToLower()}%";
                query = query.Where(x =>
                    EF.Functions.Like(x.NameEn.ToLower(), searchTerm) ||
                    EF.Functions.Like(x.NameAr.ToLower(), searchTerm) ||
                    EF.Functions.Like(x.Vendor.FullName.ToLower(), searchTerm) ||
                    EF.Functions.Like(x.Vendor.BusinessName ?? "".ToLower(), searchTerm) ||
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
                p.Reviews!.Any() ? (double)p.Reviews!.Average(r => r.Rating) : 0.0
            ));

            var products = await PaginatedList<ProductResponse>.CreateAsync(
                source,
                request.PageNumer,
                request.PageSize,
                cancellationToken
            );

            return Result.Success(products);

        }

        public async Task<Result> UpdateProductAsync(int id,UpdateProductRequest request,string currentUserId,bool isAdmin,CancellationToken cancellationToken = default)
        {
            var productQuery = _context.Products!.Where(p => p.Id == id);

            
            if (!isAdmin)
            {
                productQuery = productQuery.Where(p => p.VendorId == currentUserId);
            }


            var currentProduct = await productQuery.FirstOrDefaultAsync(cancellationToken);
            string? imagePath = null;

            if (currentProduct == null)
            {
                return Result.Failure(ProductErrors.NotFound);
            }



            if (request.Image != null)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "images/products");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{request.Image.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await request.Image.CopyToAsync(stream);

                imagePath = $"/images/products/{uniqueFileName}";
                currentProduct.MainImageUrl = imagePath;
            }

          
            // request.Adapt(currentProduct);
            currentProduct.NameEn = request.NameEn;
            currentProduct.NameAr = request.NameAr;
            currentProduct.Description = request.Description;
            currentProduct.Price = request.Price;
            currentProduct.UpdatedAt = DateTime.UtcNow;

          
            

            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }

        public async Task<Result> DeleteProductAsync(int id,string currentUserId,bool isAdmin,CancellationToken cancellationToken = default)
        {
            var productQuery = _context.Products!.Where(p => p.Id == id);

            // Apply vendor filter if not admin
            if (!isAdmin)
            {
                productQuery = productQuery.Where(p => p.VendorId == currentUserId);
            }

            var product = await productQuery.FirstOrDefaultAsync(cancellationToken);

            if (product == null)
            {
                return Result.Failure(ProductErrors.NotFound);
            }

            _context.Remove(product);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result<PaginatedList<ReviewResponse>>> GetReviewsForSpecificServiceAsync(int productId, RequestFilter request,CancellationToken cancellationToken = default)
        {
            
            var productExists = await _context.Products!
                .AnyAsync(x => x.Id == productId, cancellationToken);

            if (!productExists)
                return Result.Failure<PaginatedList<ReviewResponse>>(ProductErrors.ProductNotFound);

           
            var query = _context.Reviews!
                .Where(x => x.ProductId == productId)
                .Include(x => x.User)
                .Include(x => x.Product)
                    .ThenInclude(x => x.Vendor)
                .AsNoTracking();

            
            if (!string.IsNullOrEmpty(request.SearchValue))
            {
                var searchTerm = $"%{request.SearchValue.ToLower()}%";
                query = query.Where(x =>
                    EF.Functions.Like(x.Comment!.ToLower(), searchTerm) ||
                    EF.Functions.Like(x.User.FullName.ToLower(), searchTerm)
                );
            }

           
            if (request.MinRating.HasValue)
            {
                query = query.Where(x => x.Rating >= request.MinRating.Value);
            }

            if (request.MaxRating.HasValue)
            {
                query = query.Where(x => x.Rating <= request.MaxRating.Value);
            }

          

            var sortColumn = !string.IsNullOrEmpty(request.SortColumn)
                ? request.SortColumn
                : "CreatedAt";  // Default sort

            var sortDirection = !string.IsNullOrEmpty(request.SortDirection)
                ? request.SortDirection
                : "desc";  // Default direction

            query = query.OrderBy($"{sortColumn} {sortDirection}");

            
            var source = query.Select(x => new ReviewResponse(
                x.Id,
                x.Product.NameEn,
                x.Product.NameAr,
                x.User.FullName,
                x.User.Email,
                x.Product.Vendor.FullName,
                x.Rating,
                x.Comment,
                x.CreatedAt
            ));

           
            var reviews = await PaginatedList<ReviewResponse>.CreateAsync(
                source,
                request.PageNumer,
                request.PageSize,
                cancellationToken
            );

            return reviews.Items.Any()
                ? Result.Success(reviews)
                : Result.Failure<PaginatedList<ReviewResponse>>(ReviewErrors.ReviewsNotFound);
        }

        public async Task<Result<ReviewResponse>> AddReviewAsync(int productId,string userId, ReviewRequest request, CancellationToken cancellationToken = default)
        {
            var productExisit = await _context.Products!.AnyAsync(x => x.Id == productId, cancellationToken);

            if (!productExisit)
                return Result.Failure<ReviewResponse>(ProductErrors.ProductNotFound);

            bool hasOrdered = await _context.OrderProducts!
                 .AnyAsync(op =>
                     op.ProductId == productId &&
                     op.Order.ApplicationUserId == userId,
                     cancellationToken
                 );

            if (!hasOrdered)
                return Result.Failure<ReviewResponse>(ReviewErrors.HasNotOrdered);

            // 2. Check if there's a DELIVERED shipment for this product
            bool hasDeliveredShipment = await _context.OrderProducts!
                .Where(op =>
                    op.ProductId == productId &&
                    op.Order.ApplicationUserId == userId)
                .AnyAsync(op =>
                    _context.Shippings!.Any(s =>
                        s.OrderId == op.OrderId &&
                        s.VendorId == op.Product.VendorId &&
                        s.Status == ShippingStatus.Delivered
                    ),
                    cancellationToken
                );

            if (!hasDeliveredShipment)
                return Result.Failure<ReviewResponse>(ReviewErrors.ShipmentNotDelivered);


            var existingReview = await _context.Reviews!
                    .FirstOrDefaultAsync(r =>
                        r.ProductId == productId &&
                        r.ApplicationUserId == userId, cancellationToken: cancellationToken
                    );

            if (existingReview != null)
                return Result.Failure<ReviewResponse>(ReviewErrors.DuplicatedReview);


            var review = new Review
            {
                ProductId = productId,
                ApplicationUserId = userId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Reviews!.AddAsync(review, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await UpdateProductAverageRating(productId);

            var savedReview = await _context.Reviews!
                .Include(r => r.Product)
                    .ThenInclude(p => p.Vendor)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == review.Id, cancellationToken);

            if (savedReview == null)
                return Result.Failure<ReviewResponse>(ReviewErrors.ReviewNotFound);

            // 8. Create response
            var response = new ReviewResponse(
                savedReview.Id,
                savedReview.Product.NameEn,
                savedReview.Product.NameAr,
                savedReview.User?.FullName ?? "Unknown",
                savedReview.User?.Email ?? "Unknown",
                savedReview.Product.Vendor?.FullName ?? "Unknown",
                savedReview.Rating,
                savedReview.Comment,
                savedReview.CreatedAt
            );

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


        public async Task<Result<List<ProductRequestCount>>> Gettop5MostCommonProductAsync(CancellationToken cancellationToken = default)
        {
            var query = _context.OrderProducts!
               .GroupBy(op => op.ProductId)
               .Select(g => new
               {
                   ProductId = g.Key,
                   OrderCount = g.Sum(op => op.Quantity)
               })
               .OrderByDescending(x => x.OrderCount).Take(5);

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

        public async Task<Result<IEnumerable<MostRequestedProductResponse>>> GetMostRequestedProductFromAVendorAsync(string vendorId,CancellationToken cancellationToken = default)
        {
            var products = await _context.Products!
                .Where(p => p.VendorId == vendorId)
                .Select(p => new {
                    Product = p,
                    OrderCount = p.OrderProducts!.Count,
                    TotalRevenue = p.OrderProducts.Sum(op => op.Quantity * op.Product.Price)
                })
                .OrderByDescending(x => x.OrderCount)
                .Select(x => new MostRequestedProductResponse(
                    x.Product.Id,
                    x.Product.NameEn,
                    x.Product.NameAr,
                    x.Product.MainImageUrl!,
                    x.OrderCount,
                    x.TotalRevenue
                ))
                .ToListAsync(cancellationToken);

            if (!products.Any())
                return Result.Failure<IEnumerable<MostRequestedProductResponse>>(ProductErrors.ProductsNotFound);

            return Result.Success(products.AsEnumerable());
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

        public async Task<Result<IEnumerable<ProductDto>>> GetAllProductsUnderSubcategoryandVendorAsync(string providerId, int subCategoryId, CancellationToken cancellationToken = default)
        {
            var products = await _context.Products
       .Where(p => p.VendorId == providerId && p.SubCategoryId == subCategoryId)
       .Select(p => new ProductDto(
           p.Id,
           p.NameEn,
           p.NameAr,
           p.MainImageUrl,
           p.Description,
           p.Price

       ))
       .AsNoTracking()
       .ToListAsync(cancellationToken);


            if (!products.Any())
                return Result.Failure<IEnumerable<ProductDto>>(ProductErrors.ProductsNotFound);

            return Result.Success(products.AsEnumerable());
        }
    }
}
