using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.Common
{
    public record RequestFilter
    {
        public int PageNumer { get; init; } = 1;
        public int PageSize { get; init; } = 10;

        public string? SearchValue { get; init; }
        public string? SortColumn { get; init; }
        public string? SortDirection { get; init; }

        public bool? Status { get; init; }

        public List<string>? Statuses { get; init; }
    }
}
