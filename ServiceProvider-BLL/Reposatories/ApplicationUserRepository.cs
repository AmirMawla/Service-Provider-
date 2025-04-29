using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SeeviceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.Common;
using ServiceProvider_BLL.Dtos.UsersDto;
using ServiceProvider_BLL.Dtos.VendorDto;
using ServiceProvider_BLL.Errors;
using ServiceProvider_BLL.Interfaces;
using ServiceProvider_DAL.Data;
using ServiceProvider_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Reposatories
{
    public class ApplicationUserRepository : BaseRepository<ApplicationUser> , IApplicationUserRepository
    {
        private readonly AppDbContext _context;

        public ApplicationUserRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Result<PaginatedList<UserResponse>>> GetAllMobileUsers(RequestFilter request , CancellationToken cancellationToken = default) 
        {
            var query = _context.ApplicationUsers!
                .AsNoTracking()
                .AsQueryable();


            if (!query.Any())
                return Result.Failure<PaginatedList<UserResponse>>(VendorErrors.NotFound);

            if (!string.IsNullOrEmpty(request.SearchValue))
            {
                query = query.Where(x =>
                     x.FullName.Contains(request.SearchValue));
            }

            if (!string.IsNullOrEmpty(request.SortColumn))
            {
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");
            }

            var source = query.ProjectToType<UserResponse>();

            var users = await PaginatedList<UserResponse>.CreateAsync(source, request.PageNumer, request.PageSize, cancellationToken);

            return Result.Success(users);
        }

        public async Task<Result<int>> GetTotalUsersCountAsync(CancellationToken cancellationToken = default) 
        {
            try
            {
                // Query the ApplicationUsers table
                var usersCount = await _context.ApplicationUsers!.CountAsync(cancellationToken);
                return Result.Success(usersCount);
            }
            catch (Exception ex)
            {
                // Log the exception here (e.g., using ILogger)
                return Result.Failure<int>(new Error("Error","Failed to retrieve user count.",StatusCodes.Status400BadRequest));
            }
        }
    }
}
