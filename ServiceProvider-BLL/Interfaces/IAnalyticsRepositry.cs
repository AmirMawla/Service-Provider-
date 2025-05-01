using SeeviceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.AnalyticsDto.cs;
using ServiceProvider_BLL.Dtos.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Interfaces
{
    public interface IAnalyticsRepositry
    {
        Task<Result<TodaysStatsResponse>> GetTodaysStatsAsync(CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<VendorRevenueResponse>>> GetTopVendorsAsync(CancellationToken cancellationToken = default);
        Task<Result<OverAllStatisticsResponse>> GetOverallStatisticsAsync();

    }
}
