using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.VendorDto
{
    public record VendorDashboardResponse(
       int OrdersThisWeek,
       int OrdersThisMonth,
       List<MonthlyRevenue> RevenueTrend,
       decimal CurrentRevenue,
       double AverageRating
    );

    public record MonthlyRevenue(
        string Month,
        decimal Amount
    );
}
