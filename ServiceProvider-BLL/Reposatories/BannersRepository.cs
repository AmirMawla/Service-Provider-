using Microsoft.EntityFrameworkCore;
using Mapster;
using SeeviceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.AnalyticsDto.cs;
using ServiceProvider_BLL.Interfaces;
using ServiceProvider_DAL.Data;
using ServiceProvider_DAL.Entities;
using System;
using System.Collections.Generic;
using ServiceProvider_BLL.Dtos.BannersDto;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceProvider_BLL.Dtos.ProductDto;
using ServiceProvider_BLL.Dtos.VendorDto;
using ServiceProvider_BLL.Dtos.OrderDto;
using ServiceProvider_BLL.Errors;
using Stripe.Climate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ServiceProvider_BLL.Reposatories
{
    public class BannersRepository : BaseRepository<Banners>, IBannersRepository
    {
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment _env;
        public BannersRepository(AppDbContext _context , IWebHostEnvironment env) : base(_context)
        {
            _appDbContext = _context;
            _env = env;
        }


        public async Task<Result<BannerResponse2>> AddBannerAsync(string vendorId ,BannerRequest2 request, CancellationToken cancellationToken = default)
        {
            var product = await _appDbContext.Products!
        .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.VendorId == vendorId, cancellationToken);

            if (product == null)
                return Result.Failure<BannerResponse2>(BannerErrors.DontoWnThisProduct);


            string? imagePath = null;

            if (request.ImageUrl != null)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "images/banners");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{request.ImageUrl.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await request.ImageUrl.CopyToAsync(stream);

                imagePath = $"/images/banners/{uniqueFileName}";
            }


            var banner = new Banners
            {
                ProductId = request.ProductId,
                VendorId = vendorId,
                Description = request.Description,
                ImageUrl = imagePath!,
                DiscountPercentage = request.DiscountPercentage,
                DiscountCode = request.DiscountCode
            };

            _appDbContext.Banners!.Add(banner);
            await _appDbContext.SaveChangesAsync(cancellationToken);

            return Result.Success(banner.Adapt<BannerResponse2>());
        }

        public async Task<Result> DeleteBannerAsync(string vendorId, int productid, decimal DiscountPercentege, CancellationToken cancellationToken = default)
        {
            var banner =await _appDbContext.Banners!.FirstOrDefaultAsync(b => b.VendorId == vendorId && b.ProductId == productid && b.DiscountPercentage == DiscountPercentege, cancellationToken);
            if (banner == null)
                return Result.Failure(BannerErrors.DontoWnThisProduct);

            _appDbContext.Banners!.Remove(banner);
            await _appDbContext.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }


        public async Task<Result<decimal>> GetCartDiscountByCodeAsync(string discountCode,string UserId,CancellationToken cancellationToken = default)
        {
            var totalDiscount = await _appDbContext.CartProducts!
                .Include(cp => cp.Product)!
                .ThenInclude(p => p.Banners)
        .Where(cp => cp.Cart.ApplicationUserId == UserId)
        .Select(cp => new
        {
            Price = cp.Product!.Price,
            Quantity = cp.Quantity,
            VendorId = cp.Product.VendorId,
            Banner = cp.Product.Banners!
                .FirstOrDefault(b =>
                    b.DiscountCode != null &&
                    b.DiscountCode.ToLower() == discountCode.ToLower() &&
                    b.VendorId == cp.Product.VendorId 
                )
        })
        .Where(x => x.Banner != null )
        .SumAsync(x => (x.Price * x.Quantity) * (x.Banner!.DiscountPercentage! / 100), cancellationToken);



            return Result.Success(totalDiscount);
        }

        public async Task<Result<IEnumerable<BannersResponse>>> GetTopBannersAsync(CancellationToken cancellationToken = default)
        {

            var topBanners = await _appDbContext.Banners!
               .Include(b => b.Product)
               .Include(b => b.Vendor)
               .OrderByDescending(b => b.DiscountPercentage)
               .Take(5)
               .Select(b => new BannersResponse
               (
                   b.DiscountCode!,
                   b.Description,
                   b.ImageUrl,
                   b.DiscountPercentage,
                   new VendorBannersResponse
                   (
                       b.Vendor!.Id,
                       b.Vendor.FullName,
                       b.Vendor.BusinessName,
                       b.Vendor.BusinessType,
                       b.Vendor.TaxNumber,
                       b.Vendor.Rating,
                       b.Vendor.ProfilePictureUrl,
                       b.Vendor.CoverImageUrl
                   ),
                   new ProductBannersResponse
                   (
                       b.Product!.Id,
                       b.Product.NameEn,
                       b.Product.NameAr,
                       b.Product.Description,
                       b.Product.Price,
                       b.Product.MainImageUrl
                       
                   )
               ))
               .ToListAsync(cancellationToken); // Execute calculation in-memory


            return Result.Success(topBanners.Adapt<IEnumerable<BannersResponse>>());
        }

        public async Task<Result<IEnumerable<BannerResponse2>>> GetVendorBannersAsync(string vendorId, CancellationToken cancellationToken = default)
        {


            var banners = await _appDbContext.Banners!
                   .Where(b => b.VendorId == vendorId)
                   .Select(b => new BannerResponse2(
                                 b.ProductId,
                                 b.Description,
                                 b.ImageUrl,
                                 b.DiscountPercentage,
                                 b.DiscountCode
                           )).ToListAsync(cancellationToken);

            return banners.Any()
                ? Result.Success(banners.Adapt<IEnumerable<BannerResponse2>>())
                : Result.Failure<IEnumerable<BannerResponse2>>(BannerErrors.NoOffersForThisVendor);

        }


        public async Task<Result<BannerResponse2>> UpdateBannerAsync(string vendorId, BannerRequest2 request, CancellationToken cancellationToken = default)
        {
            var banner = await _appDbContext.Banners!.Where(b => b.VendorId == vendorId &&
            b.ProductId == request.ProductId &&
            b.DiscountPercentage == request.DiscountPercentage).FirstOrDefaultAsync(cancellationToken);

            if (banner == null)
                Result.Failure<BannerResponse2>(BannerErrors.DontoWnThisProduct);

            string? imagePath = null;

            if (request.ImageUrl != null)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "images/banners");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{request.ImageUrl.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await request.ImageUrl.CopyToAsync(stream);

                imagePath = $"/images/banners/{uniqueFileName}";
                banner!.ImageUrl = imagePath;
            }



            banner!.Description = request.Description;
            banner.DiscountCode = request.DiscountCode;

            _appDbContext.Banners!.Update(banner);
            await _appDbContext.SaveChangesAsync(cancellationToken);

            return Result.Success(banner.Adapt<BannerResponse2>());
        }
    }
}
