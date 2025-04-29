using Azure.Core;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SeeviceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.Common;
using ServiceProvider_BLL.Dtos.ProductDto;
using ServiceProvider_BLL.Dtos.VendorDto;
using ServiceProvider_BLL.Errors;
using ServiceProvider_BLL.Interfaces;
using ServiceProvider_DAL.Data;
using ServiceProvider_DAL.Entities;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ServiceProvider_BLL.Reposatories
{
    public class VendorRepository : BaseRepository<Vendor>, IVendorRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<Vendor> _userManager;
        private readonly IPasswordHasher<Vendor> _passwordHasher;
        public VendorRepository(AppDbContext context, UserManager<Vendor> userManager, IPasswordHasher<Vendor> passwordHasher) : base(context)
        {
            _context = context;
            _userManager = userManager;
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<IEnumerable<VendorResponse>>> GetAllProviders(CancellationToken cancellationToken = default)
        {
            var approvedVendors = await _context.Users
                .Where(x => x.IsApproved)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken);

            if (!approvedVendors.Any())
                return Result.Failure<IEnumerable<VendorResponse>>(VendorErrors.NotFound);

            var notAdmins = new List<Vendor>();
            foreach (var vendor in approvedVendors)
            {
                if (await _userManager.IsInRoleAsync(vendor, "Vendor"))
                {
                    notAdmins.Add(vendor);
                }
            }

            var providers = notAdmins.Adapt<IEnumerable<VendorResponse>>();

            return Result.Success(providers);
        }

        public async Task<Result<PaginatedList<VendorRatingResponse>>> GetVendorsRatings(RequestFilter request,CancellationToken cancellationToken = default) 
        {

            var query = _context.Users
                .Where(x => x.IsApproved);

            if (!query.Any())
                return Result.Failure<PaginatedList<VendorRatingResponse>>(VendorErrors.NotFound);

            if (!string.IsNullOrEmpty(request.SearchValue))
            {
                query = query.Where(x =>
                     x.FullName.Contains(request.SearchValue) ||
                     (x.BusinessName ?? "").Contains(request.SearchValue) ||
                     (x.BusinessType ?? "").Contains(request.SearchValue));
            }


            if (!string.IsNullOrEmpty(request.SortColumn))
            {
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");
            }

            var source = query.ProjectToType<VendorRatingResponse>().AsNoTracking();

            var vendors = await PaginatedList<VendorRatingResponse>.CreateAsync(source, request.PageNumer, request.PageSize);

            return Result.Success(vendors);
        }

        public async Task<Result<VendorResponse>> GetProviderDetails(string providerId, CancellationToken cancellationToken = default)
        {
            var provider = await _context.Users.AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == providerId, cancellationToken);

            if (provider == null)
                return Result.Failure<VendorResponse>(VendorErrors.NotFound);

            return Result.Success(provider.Adapt<VendorResponse>());
        }

        public async Task<Result<IEnumerable<ProductsOfVendorDto>>> GetProviderMenuAsync(string providerId, CancellationToken cancellationToken)
        {
            var providerExists = await _context.Users.AnyAsync(u => u.Id == providerId, cancellationToken);
            if (!providerExists)
                return Result.Failure<IEnumerable<ProductsOfVendorDto>>(VendorErrors.NotFound);

            var menu = await _context.Products!
                .Where(p => p.VendorId == providerId)
                .Select(p => new ProductsOfVendorDto(
                     p.Id,
                     p.NameEn,
                     p.NameAr,
                     p.Description!,
                     p.MainImageUrl,
                     p.Price,
                     p.SubCategory.Category.NameEn,
                     p.SubCategory.Category.NameAr
                ))
                .ToListAsync(cancellationToken);

            return menu.Any()
                ? Result.Success<IEnumerable<ProductsOfVendorDto>>(menu)
                : Result.Failure<IEnumerable<ProductsOfVendorDto>>(ProductErrors.NotFound);
        }
       

        public async Task<Result<UpdateVendorResponse>> UpdateVendorAsync(string id,UpdateVendorResponse vendorDto, CancellationToken cancellationToken = default)
        {

            var vendor = await _userManager.FindByIdAsync(id);
            if (vendor == null )
                return Result.Failure<UpdateVendorResponse>(VendorErrors.NotFound);
            
            vendor.UserName = vendorDto.UserName;
            if (vendor.FullName == null)
                vendor.FullName = vendorDto.UserName;

            vendor.BusinessName = vendorDto.BusinessName;
            vendor.CoverImageUrl = vendorDto.CoverImageUrl;
            vendor.ProfilePictureUrl = vendorDto.ProfilePictureUrl;
            var result = await _userManager.UpdateAsync(vendor);
            await _context.SaveChangesAsync();
            return Result.Success(vendor.Adapt<UpdateVendorResponse>());
        }

        public async Task<Result> ChangeVendorPasswordAsync(string vendorId, ChangeVendorPasswordRequest request)
        {

            var vendor = await _userManager.FindByIdAsync(vendorId);
            var result = await _userManager.ChangePasswordAsync(vendor!, request.CurrentPassword, request.NewPassword);
            if (result.Succeeded)
                return Result.Success();

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }


        public async Task<Result> DeleteVendorAsync(string vendorId , CancellationToken cancellationToken = default) 
        {
            var vendor = await _context.Users.FirstOrDefaultAsync(v => v.Id == vendorId, cancellationToken);
            if (vendor == null)
                return Result.Failure(VendorErrors.NotFound);

             _context.Users.Remove(vendor);

            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        public async Task<Result<IEnumerable<VendorResponse>>> GetPendingVendorsAsync(CancellationToken cancellationToken = default)
        {
            var notApprovedUsers = await _userManager.Users
             .Where(u => !u.IsApproved)
             .ToListAsync(cancellationToken);

            // Then, filter in memory for those who are in the Vendor role
            var pendingVendors = new List<Vendor>();
            foreach (var vendor in notApprovedUsers)
            {
                if (await _userManager.IsInRoleAsync(vendor, "Vendor"))
                {
                    pendingVendors.Add(vendor);
                }
            }

            // Project to VendorResponse after filtering
            var vendorResponses = pendingVendors.Adapt<IEnumerable<VendorResponse>>();

            if (!vendorResponses.Any())
                return Result.Failure<IEnumerable<VendorResponse>>(VendorErrors.NoPendingVendors);

            return Result.Success(vendorResponses);
        }

        public async Task<Result> ApproveVendorAsync(string vendorId, CancellationToken cancellationToken = default)
        {
            var vendor = await _userManager.FindByIdAsync(vendorId);
            if (vendor == null || !(await _userManager.IsInRoleAsync(vendor, "Vendor")))
                return Result.Failure(VendorErrors.NotFound);

            if (vendor.IsApproved && vendor.EmailConfirmed)
                return Result.Failure(VendorErrors.ApprovedVendor);

            vendor.IsApproved = true;
            vendor.EmailConfirmed = true;

            var result = await _userManager.UpdateAsync(vendor);
            if (result.Succeeded)
                return Result.Success();

            var error = result.Errors.First();

            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

        }

    }
}
