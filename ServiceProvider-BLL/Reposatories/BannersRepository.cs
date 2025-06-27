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

namespace ServiceProvider_BLL.Reposatories
{
    public class BannersRepository : BaseRepository<Banners>, IBannersRepository
    {
        private readonly AppDbContext _appDbContext;
        public BannersRepository(AppDbContext _context) : base(_context)
        {
            _appDbContext = _context;
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
        .Where(x => x.Banner != null && x.Banner.DiscountPercentage.HasValue)
        .SumAsync(x => (x.Price * x.Quantity) * (x.Banner!.DiscountPercentage!.Value / 100), cancellationToken);



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
                   b.DiscountCode,
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
    }
}
