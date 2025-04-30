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
        public async Task<Result<IEnumerable<BannersResponse>>> GetTopBannersAsync(CancellationToken cancellationToken = default)
        {

            var topBanners = await _appDbContext.Banners
               .Include(b => b.Product)
               .Include(b => b.Vendor)
               .OrderByDescending(b => b.DiscountPercentage)
               .Take(5)
               .Select(b => new BannersResponse
               (
                   b.Id,
                   b.Description,
                   b.ImageUrl,
                   b.DiscountPercentage,
                   new VendorBannersResponse
                   (
                       b.Vendor.Id,
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
                       b.Product.Id,
                       b.Product.NameEn,
                       b.Product.NameAr,
                       b.Product.Description,
                       b.Product.Price,
                       b.Product.MainImageUrl
                       
                   )
               ))
               .ToListAsync(); // Execute calculation in-memory


            return Result.Success(topBanners.Adapt<IEnumerable<BannersResponse>>());
        }
    }
}
