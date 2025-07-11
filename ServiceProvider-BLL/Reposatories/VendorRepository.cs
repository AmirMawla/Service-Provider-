﻿using Azure.Core;
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
using Government.Contracts.AccountProfile.cs;
using NotificationService.Models;
using System.Security.Cryptography;
using MassTransit;


namespace ServiceProvider_BLL.Reposatories
{
    public class VendorRepository : BaseRepository<Vendor>, IVendorRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Vendor> _userManager;
        private readonly IPasswordHasher<Vendor> _passwordHasher;
        private readonly string _vendorId;
        private readonly IWebHostEnvironment _env;
        private readonly IPublishEndpoint _publishEndpoint;
        private const int OtpLength = 6;                     // طول الرمز
        private static readonly TimeSpan OtpTtl = TimeSpan.FromMinutes(10);

        public VendorRepository(AppDbContext context, UserManager<Vendor> userManager, IPasswordHasher<Vendor> passwordHasher, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env, IPublishEndpoint publishEndpoint) : base(context)
        {
            _context = context;
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _vendorId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            _env = env;
            _publishEndpoint = publishEndpoint;
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

        public async Task<Result<VendorResponse>> GetProviderProfile(string providerId, CancellationToken cancellationToken = default)
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
                .Where(op => op.Product.VendorId == vendorId && op.Order.Payment.Status == PaymentStatus.Completed)
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

            // حساب أول يوم في الأسبوع (الإثنين)
            var startOfWeek = now.Date.AddDays(-(int)(now.DayOfWeek == DayOfWeek.Sunday ? 6 : now.DayOfWeek - DayOfWeek.Monday));

            // حساب أول يوم في الشهر
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            // جيب الطلبات الخاصة بالبائع
            var orderIdsOfVendor = await _context.OrderProducts
                .Where(op => op.Product.VendorId == _vendorId)
                .Select(op => op.OrderId)
                .Distinct()
                .ToListAsync(cancellationToken);

            // جيب تفاصيل الطلبات دفعة واحدة
            var vendorOrders = await _context.Orders!
                .Where(o => orderIdsOfVendor.Contains(o.Id) && o.OrderDate >= startOfMonth)
                .Select(o => new { o.Id, o.OrderDate })
                .ToListAsync(cancellationToken);

            var weekly = vendorOrders.Count(o => o.OrderDate >= startOfWeek);
            var monthly = vendorOrders.Count; // لأننا مصفيينهم من البداية بـ startOfMonth

            return (weekly, monthly);
        }

        private async Task<(List<MonthlyRevenue> trend, decimal current)> GetRevenueData(CancellationToken cancellationToken = default)
        {


            var paymentsQuery = _context.OrderProducts
                .Where(op => op.Product.VendorId == _vendorId);

            // Last 4 months revenue trend
            var trend = await paymentsQuery
                .Where(op => op.Order.Payment.Status == PaymentStatus.Completed)
                .GroupBy(op => new { op.Order.Payment.TransactionDate.Year, op.Order.Payment.TransactionDate.Month })
                .OrderByDescending(g => g.Key.Year)
                .ThenByDescending(g => g.Key.Month)
                .Take(4)
                .Select(g => new MonthlyRevenue(
                     CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(g.Key.Month),
                      g.Sum(op => op.Quantity * op.Product.Price )
                ))
                .ToListAsync(cancellationToken);

            // Current total revenue
            var current = await paymentsQuery
                .Where(op => op.Order.Payment.Status == PaymentStatus.Completed)
                .SumAsync(op => op.Quantity * op.Product.Price, cancellationToken);

            return (trend, current);
        }

        private async Task<double> GetAverageRating(CancellationToken cancellationToken = default)
        {


            return await _context.Reviews!
                .Where(r => r.Product.VendorId == _vendorId)
                .AverageAsync(r => (double?)r.Rating,cancellationToken) ?? 0.0;
        }



        public async Task<Result> UpdateVendorAsync(string id,UpdateVendorRequest vendorDto, CancellationToken cancellationToken = default)
        {

            var vendor = await _userManager.FindByIdAsync(id);
            if (vendor == null )
                return Result.Failure<UpdateVendorRequest>(VendorErrors.NotFound);

            if (!string.IsNullOrWhiteSpace(vendorDto.FullName))
                vendor.FullName = vendorDto.FullName;

            if (!string.IsNullOrWhiteSpace(vendorDto.BusinessName))
                vendor.BusinessName = vendorDto.BusinessName;

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

        public async Task<Result> GenerateAndSendAsync(string email, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return Result.Success();

            // 1) إنشاء رمز عشوائي
            var otp = RandomNumberGenerator
                        .GetInt32((int)Math.Pow(10, OtpLength - 1),
                                  (int)Math.Pow(10, OtpLength))
                        .ToString();

            // logger.LogInformation("OTP for {Email} is {Otp}", email, otp); // 🔐 لا تنس حذفه لاحقًا!
            Console.WriteLine($"[DEBUG] OTP for {email} is: {otp}");

            // 2) تشفيره
            var hash = BCrypt.Net.BCrypt.HashPassword(otp);


            // 3) احذف أية رموز قديمة لهذا الإيميل
            await _context.OtpEntries
                     .Where(e => e.Email == email)
                     .ExecuteDeleteAsync(ct);

            // 4) خزّن السطر الجديد
            await _context.OtpEntries.AddAsync(new OtpEntry
            {
                Email = email,
                HashedOtp = hash,
                Expiry = DateTime.UtcNow.Add(OtpTtl)
            }, ct);
            await _context.SaveChangesAsync(ct);



            // 5) ✉ نشر إشعار «إيميل» عبر MassTransit → RabbitMQ
            //var notification = new NotificationMessage
            //{
            //    Title = "رمز التحقق لاستعادة كلمة المرور",
            //    Body = $"رمزك هو: {otp}. صالح لمدة {OtpTtl.TotalMinutes} دقيقة.",
            //    Type = NotificationType.UserSpecific,
            //    Channels = new() { ChannelType.Email },
            //    TargetUsers = new() { user.Id! },             
            //    Category = NotificationCategory.Alert
            //};

            //var evt = new NotificationMessage
            //{
            //    Title = "رمز التحقق لاستعادة كلمة المرور",
            //    Body = $"رمزك هو: {otp}. صالح لمدة {OtpTtl.TotalMinutes} دقيقة.",
            //    Type = NotificationType.Group,
            //    Channels = new List<ChannelType> { ChannelType.Email },
            //    TargetUsers = new List<string> { "g1623g6-12g31g-123g-123g-123g123g", "g1623g6-12g31g-123g-123g-123g123g" },
            //    Category = NotificationCategory.Update
            //};

            //await _publishEndpoint.Publish(evt, ctx =>
            //{
            //    ctx.SetRoutingKey("user.notification.created");
            //});


            return Result.Success();
        }


        public async Task<Result<VerifyResponse>> VerifyAsync(string email, string otp, CancellationToken ct = default)
        {
            // 1) جلب آخر رمز لم ينتهِ بعد
            var entry = await _context.OtpEntries
                .Where(e => e.Email == email && e.Expiry > DateTime.UtcNow)
                .OrderByDescending(e => e.Expiry)
                .FirstOrDefaultAsync(ct);

            // 2) تحقق من التوافق
            if (entry is null || !(BCrypt.Net.BCrypt.Verify(otp, entry.HashedOtp)))
                return Result.Failure<VerifyResponse>(new Error("Invalid Otp","this otp is invalid",StatusCodes.Status400BadRequest));

            // 3) حذف السطر (One-time use)
            _context.OtpEntries.Remove(entry);
            await _context.SaveChangesAsync(ct);

            // 4) توليد ResetPasswordToken رسمي من Identity
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return Result.Failure<VerifyResponse>(VendorErrors.NotFound);

            var PasswordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            return Result.Success(new VerifyResponse(PasswordResetToken));
        }


        public async Task<Result> ResetUserPassword(string Email, string ResetToken, string NewPassword, CancellationToken ct = default)
        {

            var user = await _userManager.FindByEmailAsync(Email);

            if (user is null)
                return Result.Failure(VendorErrors.NotFound);

            var result = await _userManager.ResetPasswordAsync(user, ResetToken, NewPassword);

            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure(new Error(error.Code, error.Description,StatusCodes.Status400BadRequest));

            }

            return Result.Success();


        }


    }
}
