using SeeviceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.Common;
using ServiceProvider_BLL.Dtos.UsersDto;
using ServiceProvider_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Interfaces
{
    public interface IApplicationUserRepository : IBaseRepository<ApplicationUser>
    {
        Task<Result<PaginatedList<UserResponse>>> GetAllMobileUsers(RequestFilter request, CancellationToken cancellationToken = default);
        Task<Result<int>> GetTotalUsersCountAsync(CancellationToken cancellationToken = default);
    }
}
