using SeeviceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.AnalyticsDto.cs;
using ServiceProvider_BLL.Dtos.BannersDto;
using ServiceProvider_BLL.Dtos.Common;
using ServiceProvider_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Interfaces
{
    public interface IBannersRepository : IBaseRepository<Banners>
    {
        Task<Result<IEnumerable<BannersResponse>>> GetTopBannersAsync(CancellationToken cancellationToken = default);
        Task<Result<PaginatedList<BannerResponse2>>> GetVendorBannersAsync(string vendorId, RequestFilter request, CancellationToken cancellationToken = default);
        Task<Result<BannerResponse2>> AddBannerAsync(string vendorId ,BannerRequest2 request, CancellationToken cancellationToken = default);
        Task<Result> DeleteBannerAsync(string vendorId,int productid, decimal DiscountPercentege, CancellationToken cancellationToken = default);
        Task<Result<BannerResponse2>> UpdateBannerAsync(string vendorId, BannerRequest2 request, CancellationToken cancellationToken = default);
        Task<Result<decimal>> GetCartDiscountByCodeAsync(string discountCode,string UserId,CancellationToken cancellationToken = default);
    }
}
