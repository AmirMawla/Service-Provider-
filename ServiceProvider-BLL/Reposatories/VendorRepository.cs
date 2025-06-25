using Azure.Core;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SeeviceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.Common;
using ServiceProvider_BLL.Dtos.ProductDto;
using ServiceProvider_BLL.Dtos.VendorDto;
using ServiceProvider_BLL.Errors;
using ServiceProvider_BLL.Interfaces;
using ServiceProvider_DAL.Data;
using ServiceProvider_DAL.Entities;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Security.Claims;
using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using ServiceProvider_BLL.Dtos.ReviewDto;


namespace ServiceProvider_BLL.Reposatories
{
    public class VendorRepository : BaseRepository<Vendor>, IVendorRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Vendor> _userManager;
        private readonly IPasswordHasher<Vendor> _passwordHasher;
        private readonly string _vendorId;
        private readonly IWebHostEnvironment _env;

        public VendorRepository(AppDbContext context, UserManager<Vendor> userManager, IPasswordHasher<Vendor> passwordHasher, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env) : base(context)
        {
            _context = context;
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _vendorId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            _env = env;
        }

        public async Task<Result<PaginatedList<VendorResponse>>> GetAllProviders(RequestFilter request,CancellationToken cancellationToken = default)
        {
            var query =  _context.Users
                .Where(x => x.UserName != "admin")
                .AsNoTracking();

            if (!query.Any())
                return Result.Failure<PaginatedList<VendorResponse>>(VendorErrors.NotFound);

            if (request.Status.HasValue)
            {
                query = query.Where(u => u.IsApproved == request.Status.Value);
            }

            if (!string.IsNullOrEmpty(request.SearchValue))
            {
                var searchTerm = $"%{request.SearchValue.ToLower()}%";
                query = query.Where(x =>
                    EF.Functions.Like(x.FullName.ToLower(), searchTerm) ||
                    EF.Functions.Like(x.BusinessName ?? "".ToLower(), searchTerm) ||
                    EF.Functions.Like(x.BusinessType ?? "".ToLower(), searchTerm)
                );
            }


            if (!string.IsNullOrEmpty(request.SortColumn))
            {
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");
            }

            var source = query.ProjectToType<VendorResponse>().AsNoTracking();


            var vendors = await PaginatedList<VendorResponse>.CreateAsync(
                source,
                request.PageNumer,
                request.PageSize,
                cancellationToken
            );

            return Result.Success(vendors);
        }

        public async Task<Result<PaginatedList<VendorResponse>>> GetAllProvidersForMobile(RequestFilter request, CancellationToken cancellationToken = default)
        {
            var query = _context.Users
                .Where(x => x.UserName!.ToLower() != "amirmawlaa" && x.IsApproved)
                .AsNoTracking();

            if (!query.Any())
                return Result.Failure<PaginatedList<VendorResponse>>(VendorErrors.NotFound);

            if (!string.IsNullOrEmpty(request.SearchValue))
            {
                var searchTerm = $"%{request.SearchValue.ToLower()}%";
                query = query.Where(x =>
                    EF.Functions.Like(x.FullName.ToLower(), searchTerm) ||
                    EF.Functions.Like(x.BusinessName ?? "".ToLower(), searchTerm) ||
                    EF.Functions.Like(x.BusinessType ?? "".ToLower(), searchTerm)
                );
            }

            if (request.BusinessTypes != null && request.BusinessTypes.Any())
            {
                query = query.Where(x =>
                request.BusinessTypes.Any(bt =>
                    x.BusinessType != null &&
                    x.BusinessType.ToLower().Contains(bt.ToLower())
                ));
            }


            if (!string.IsNullOrEmpty(request.SortColumn))
            {
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");
            }

            var source = query.ProjectToType<VendorResponse>().AsNoTracking();


            var vendors = await PaginatedList<VendorResponse>.CreateAsync(
                source,
                request.PageNumer,
                request.PageSize,
                cancellationToken
            );

            return Result.Success(vendors);
        }

        public async Task<Result<PaginatedList<VendorRatingResponse>>> GetVendorsRatings(string vendorId ,RequestFilter request,CancellationToken cancellationToken = default) 
        {

            var vendorExists = await _context.Users.AnyAsync(u => u.Id == vendorId && u.IsApproved, cancellationToken: cancellationToken);
            if (!vendorExists) return Result.Failure<PaginatedList<VendorRatingResponse>>(VendorErrors.NotFound);

            var query = _context.Reviews!
                .Where(r => r.Product.VendorId == vendorId)
                .Include(r => r.Product)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .AsNoTracking();

            // Search filter
            if (!string.IsNullOrEmpty(request.SearchValue))
            {
                var searchTerm = $"%{request.SearchValue.ToLower()}%";
                query = query.Where(r =>
                    EF.Functions.Like(r.Comment!.ToLower(), searchTerm) ||
                    EF.Functions.Like(r.Product.NameEn.ToLower(), searchTerm) ||
                    EF.Functions.Like(r.Product.NameAr.ToLower(), searchTerm));
            }

            // Sorting
            if (!string.IsNullOrEmpty(request.SortColumn))
            {
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");
            }

            var source = query.Select(r => new VendorRatingResponse(
                r.Id,
                r.Rating,
                r.Comment ?? string.Empty,
                r.CreatedAt,
                r.Product.NameEn,
                r.Product.NameAr,
                r.User.FullName,
                r.User.Id
            ));

            var reviews = await PaginatedList<VendorRatingResponse>.CreateAsync(
                source,
                request.PageNumer,
                request.PageSize,
                cancellationToken
            );

            return reviews.Items.Any()
            ? Result.Success(reviews)
                : Result.Failure<PaginatedList<VendorRatingResponse>>(ReviewErrors.ReviewsNotFound);
        }

        public async Task<Result<VendorResponse>> GetProviderDetails(string providerId, CancellationToken cancellationToken = default)
        {
            var provider = await _context.Users.AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == providerId, cancellationToken);

            if (provider == null)
                return Result.Failure<VendorResponse>(VendorErrors.NotFound);

            var NumOfReviews = _context.Reviews!
               .Where(r => r.Product.VendorId == providerId)
               .Count();

            var VendorDetails = new VendorResponse(
                providerId,
                provider.FullName,
                provider.Email!,
                provider.ProfilePictureUrl,
                provider.CoverImageUrl,
                provider.BusinessName,
                provider.BusinessType,
                provider.TaxNumber,
                provider.Rating,
                provider.IsApproved,
                NumOfReviews
                );




            return Result.Success(VendorDetails);
        }

        public async Task<Result<IEnumerable<ProductsOfVendorDto>>> GetProviderMenuAsync(string providerId, CancellationToken cancellationToken)
        {
            var providerExists = await _context.Users.AnyAsync(u => u.Id == providerId, cancellationToken);
            if (!providerExists)
                return Result.Failure<IEnumerable<ProductsOfVendorDto>>(VendorErrors.NotFound);

            var menu = await _context.Products!
                .Where(p => p.VendorId == providerId)
                .Select(p => new ProductsOfVendorDto(
                     p.Id,
                     p.NameEn,
                     p.NameAr,
                     p.Description!,
                     p.MainImageUrl!,
                     p.Price,
                     p.SubCategory.Category.NameEn,
                     p.SubCategory.Category.NameAr
                ))
                .ToListAsync(cancellationToken);

            return menu.Any()
                ? Result.Success<IEnumerable<ProductsOfVendorDto>>(menu)
                : Result.Failure<IEnumerable<ProductsOfVendorDto>>(ProductErrors.NotFound);
        }


        public async Task<Result<IEnumerable<TopVendorResponse>>> GetTopVendorsByOrders(CancellationToken cancellationToken = default)
        {
            var vendors = await _context.Users
                .Where(u => u.IsApproved) // Filter for vendors
                .Select(v => new {
                    Vendor = v,
                    OrderCount = v.Products!
                        .SelectMany(p => p.OrderProducts!)
                        .Count(),
                    Category = v.VendorSubCategories!
                        .Select(vsc => vsc.SubCategory.Category.NameEn)
                        .FirstOrDefault() ?? "General"
                })
                .OrderByDescending(x => x.OrderCount)
                .Take(5)
                .Select(x => new TopVendorResponse(
                    x.Vendor.Id,
                    x.Vendor.FullName,
                    x.Vendor.BusinessName!,
                    x.Vendor.BusinessType,
                    x.Vendor.ProfilePictureUrl ,
                    x.Vendor.CoverImageUrl ,
                    x.Vendor.Rating,
                    x.Category,
                    x.OrderCount
                ))
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            if (!vendors.Any())
                return Result.Failure<IEnumerable<TopVendorResponse>>(VendorErrors.NotFound);

            return Result.Success(vendors.AsEnumerable());
        }

        public async Task<Result<IEnumerable<TopFiveProductsWithVendorResponse>>> GetTopFiveProductsWithVendor(string vendorId, CancellationToken cancellationToken = default)
        {
            // Step 1: Get aggregated sales data
            var salesData = await _context.OrderProducts!
                .Where(op => op.Product.VendorId == vendorId)
                .GroupBy(op => op.ProductId)
                .Select(g => new {
                    ProductId = g.Key,
                    Sold = g.Sum(op => op.Quantity)
                })
                .OrderByDescending(x => x.Sold)
                .Take(5)
                .ToListAsync(cancellationToken);

            if (!salesData.Any())
                return Result.Failure<IEnumerable<TopFiveProductsWithVendorResponse>>(
                    new Error("Not Found", "No items Found", StatusCodes.Status404NotFound));

            // Step 2: Get product details
            var productIds = salesData.Select(s => s.ProductId).ToList();
            var products = await _context.Products!
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync(cancellationToken);

            // Step 3: Combine results
            var result = salesData.Select(s =>
            {
                var product = products.First(p => p.Id == s.ProductId);
                return new TopFiveProductsWithVendorResponse(
                    product.Id,
                    product.NameEn,
                    product.NameAr,
                    s.Sold,
                    product.Price * s.Sold
                );
            })
            .OrderByDescending(r => r.Sold)
            .ToList();

            return Result.Success(result.AsEnumerable());
        }

        public async Task<Result<VendorBusinessTypeRespons>> GetAllVendorsBusinessTypes(CancellationToken cancellationToken = default) 
        {
            var vendors = await _context.Users
                .Where(x => x.IsApproved && x.UserName != "amirmawlaa")
                .AsNoTracking()
                .ToListAsync(cancellationToken);
                

            var businessTypes = vendors.Select(x => x.BusinessType).Distinct().ToList();

            var response = new VendorBusinessTypeRespons(
                businessTypes
            );

            return Result.Success(response);
        }
        public async Task<VendorDashboardResponse> GetVendorDashboard(CancellationToken cancellationToken = default)
        {
            var ordersResult = await GetOrderCounts(cancellationToken);
            var revenueResult = await GetRevenueData(cancellationToken);
            var ratingResult = await GetAverageRating(cancellationToken);

            return new VendorDashboardResponse(
                OrdersThisWeek: ordersResult.weekly,
                OrdersThisMonth: ordersResult.monthly,
                RevenueTrend: revenueResult.trend,
                CurrentRevenue: revenueResult.current,
                AverageRating: ratingResult
            );
        }

        private async Task<(int weekly, int monthly)> GetOrderCounts(CancellationToken cancellationToken = default)
        {


            var now = DateTime.UtcNow;
            var startOfWeek = now.AddDays(-(int)now.DayOfWeek);
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            var query = _context.Orders!
                .Where(o => o.OrderProducts.Any(op =>
                    op.Product.VendorId == _vendorId));

            return (
                weekly: await query.CountAsync(o => o.OrderDate >= startOfWeek,cancellationToken),
                monthly: await query.CountAsync(o => o.OrderDate >= startOfMonth,cancellationToken)
            );
        }

        private async Task<(List<MonthlyRevenue> trend, decimal current)> GetRevenueData(CancellationToken cancellationToken = default)
        {


            var paymentsQuery = _context.Payments!
                .Where(p => p.Order.OrderProducts.Any(op =>
                    op.Product.VendorId == _vendorId));

            // Last 4 months revenue trend
            var trend = await paymentsQuery
                .GroupBy(p => new { p.TransactionDate.Year, p.TransactionDate.Month })
                .OrderByDescending(g => g.Key.Year)
                .ThenByDescending(g => g.Key.Month)
                .Take(4)
                .Select(g => new MonthlyRevenue(
                     CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key.Month),
                      g.Sum(p => p.TotalAmount)
                ))
                .ToListAsync(cancellationToken);

            // Current total revenue
            var current = await paymentsQuery.SumAsync(p => p.TotalAmount,cancellationToken);

            return (trend, current);
        }

        private async Task<double> GetAverageRating(CancellationToken cancellationToken = default)
        {


            return await _context.Reviews!
                .Where(r => r.Product.VendorId == _vendorId)
                .AverageAsync(r => (double?)r.Rating,cancellationToken) ?? 0.0;
        }


        public async Task<Result> UpdateVendorAsync(string id,UpdateVendorResponse vendorDto, CancellationToken cancellationToken = default)
        {

            var vendor = await _userManager.FindByIdAsync(id);
            if (vendor == null )
                return Result.Failure<UpdateVendorResponse>(VendorErrors.NotFound);



            string? ProfileimagePath = null;
            string? CoverimagePath = null;

            if (vendorDto.ProfilePictureUrl != null)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "images/vendors");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{vendorDto.ProfilePictureUrl.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await vendorDto.ProfilePictureUrl.CopyToAsync(stream);

                ProfileimagePath = $"/images/vendors/{uniqueFileName}";
                vendor.ProfilePictureUrl = ProfileimagePath;
            }

            if (vendorDto.CoverImageUrl != null)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "images/vendors");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{vendorDto.CoverImageUrl.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await vendorDto.CoverImageUrl.CopyToAsync(stream);

                CoverimagePath = $"/images/vendors/{uniqueFileName}";
                vendor.CoverImageUrl = CoverimagePath;
            }



            vendor.UserName = vendorDto.UserName;
            if (vendor.FullName == null)
                vendor.FullName = vendorDto.UserName;

            vendor.BusinessName = vendorDto.BusinessName;
            var result = await _userManager.UpdateAsync(vendor);
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> ChangeVendorPasswordAsync(string vendorId, ChangeVendorPasswordRequest request)
        {

            var vendor = await _userManager.FindByIdAsync(vendorId);
            var result = await _userManager.ChangePasswordAsync(vendor!, request.CurrentPassword, request.NewPassword);
            if (result.Succeeded)
                return Result.Success();

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }


        public async Task<Result> DeleteVendorAsync(string vendorId , CancellationToken cancellationToken = default) 
        {
            var vendor = await _context.Users.FirstOrDefaultAsync(v => v.Id == vendorId, cancellationToken);
            if (vendor == null)
                return Result.Failure(VendorErrors.NotFound);

             _context.Users.Remove(vendor);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result<IEnumerable<VendorResponse>>> GetPendingVendorsAsync(CancellationToken cancellationToken = default)
        {
            var notApprovedUsers = await _userManager.Users
             .Where(u => !u.IsApproved)
             .ToListAsync(cancellationToken);

            // Then, filter in memory for those who are in the Vendor role
            var pendingVendors = new List<Vendor>();
            foreach (var vendor in notApprovedUsers)
            {
                if (await _userManager.IsInRoleAsync(vendor, "Vendor"))
                {
                    pendingVendors.Add(vendor);
                }
            }

            // Project to VendorResponse after filtering
            var vendorResponses = pendingVendors.Adapt<IEnumerable<VendorResponse>>();

            if (!vendorResponses.Any())
                return Result.Failure<IEnumerable<VendorResponse>>(VendorErrors.NoPendingVendors);

            return Result.Success(vendorResponses);
        }

        public async Task<Result> ApproveVendorAsync(string vendorId, CancellationToken cancellationToken = default)
        {
            var vendor = await _userManager.FindByIdAsync(vendorId);
            if (vendor == null || !(await _userManager.IsInRoleAsync(vendor, "Vendor")))
                return Result.Failure(VendorErrors.NotFound);

            if (vendor.IsApproved && vendor.EmailConfirmed)
                return Result.Failure(VendorErrors.ApprovedVendor);

            vendor.IsApproved = true;
            vendor.EmailConfirmed = true;

            var result = await _userManager.UpdateAsync(vendor);
            if (result.Succeeded)
                return Result.Success();

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

        }
       
        public async Task<Result> DeactivateVendorAsync(string vendorId, CancellationToken cancellationToken = default)
        {
            var vendor = await _userManager.FindByIdAsync(vendorId);
            if (vendor == null || !(await _userManager.IsInRoleAsync(vendor, "Vendor")))
                return Result.Failure(VendorErrors.NotFound);

            if (vendor.IsApproved && vendor.EmailConfirmed)
            {
                vendor.IsApproved = false;
                vendor.EmailConfirmed = false;
            }

            var result = await _userManager.UpdateAsync(vendor);
            if (result.Succeeded)
                return Result.Success();

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

        }

    }
}
