using SeeviceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.AnalyticsDto.cs;
using ServiceProvider_BLL.Dtos.BannersDto;
using ServiceProvider_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Interfaces
{
    public interface IBannersRepository : IBaseRepository<Banners>
    {
        Task<Result<IEnumerable<BannersResponse>>> GetTopBannersAsync(CancellationToken cancellationToken = default);
    }
}
