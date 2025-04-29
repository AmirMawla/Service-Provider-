using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SeeviceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Authentication;
using ServiceProvider_BLL.Dtos.AuthenticationDto;
using ServiceProvider_BLL.Dtos.VendorDto;
using ServiceProvider_BLL.Errors;
using ServiceProvider_BLL.Interfaces;
using ServiceProvider_DAL.Data;
using ServiceProvider_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Reposatories
{
    public class AuthRepositry(
        UserManager<Vendor> userManager
        ,IJwtProvider jwtProvider
        ,SignInManager<Vendor> signInManager
        ,AppDbContext context) : IAuthRepositry
    {
        private readonly UserManager<Vendor> _userManager = userManager;
        private readonly IJwtProvider _jwtProvider = jwtProvider;
        private readonly SignInManager<Vendor> _signInManager = signInManager;
        private readonly AppDbContext _context = context;
        private readonly int _refreshTokenExpiryDays = 14;

        public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellation)
        {
            var vendor = await _userManager.FindByEmailAsync(email);

            if (vendor == null)
                return Result.Failure<AuthResponse>(VendorErrors.InvalidCredentials);

            var isValidPassword = await _userManager.CheckPasswordAsync(vendor, password);

            if (!isValidPassword)
                return Result.Failure<AuthResponse>(VendorErrors.InvalidCredentials);

            if (await _userManager.IsInRoleAsync(vendor, "Vendor"))
            {
                if (!vendor.IsApproved)
                    return Result.Failure<AuthResponse>(VendorErrors.NotApproved);

                if (!vendor.EmailConfirmed)
                    return Result.Failure<AuthResponse>(VendorErrors.EmailNotConfirmed);
            }

            var roles = await _userManager.GetRolesAsync(vendor);

            var (token, expiresIn) = _jwtProvider.GenerateToken(vendor, roles);

            var refreshToken = GenerateRefreshToken();

            var refreshTokenExpirationDate = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            vendor.RefreshTokens.Add(
                new RefreshToken
                {
                    Token = refreshToken,
                    ExpiresOn = refreshTokenExpirationDate
                }
                );

            await _userManager.UpdateAsync(vendor);

            var response =  new AuthResponse(vendor.Id,vendor.Email,vendor.FullName,vendor.BusinessName,vendor.BusinessType,token,expiresIn,refreshToken,refreshTokenExpirationDate);

            return Result.Success(response);
        }

        public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellation = default)
        {
            var vendorId = _jwtProvider.ValidateToken(token);

            if (vendorId is null)
                return Result.Failure<AuthResponse>(VendorErrors.InvalidCredentials);


            var vendor = await _userManager.FindByIdAsync(vendorId);

            if (vendor is null)
                return Result.Failure<AuthResponse>(VendorErrors.InvalidCredentials);


            if (!vendor.IsApproved)
                return Result.Failure<AuthResponse>(VendorErrors.NotApproved);


            var userRefreshToken = vendor.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

            if (userRefreshToken is null)
                return Result.Failure<AuthResponse>(VendorErrors.InvalidRefreshToken);


            userRefreshToken.RevokedOn = DateTime.UtcNow;

            var roles = await _userManager.GetRolesAsync(vendor);

            var (newToken, expiresIn) = _jwtProvider.GenerateToken(vendor, roles);

            var newRefreshToken = GenerateRefreshToken();

            var refreshTokenExpirationDate = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            vendor.RefreshTokens.Add(
                new RefreshToken
                {
                    Token = newRefreshToken,
                    ExpiresOn = refreshTokenExpirationDate
                }
                );

            await _userManager.UpdateAsync(vendor);

            var response = new AuthResponse(vendor.Id, vendor.Email, vendor.FullName, vendor.BusinessName, vendor.BusinessType, newToken, expiresIn, newRefreshToken, refreshTokenExpirationDate);

            return Result.Success(response);
        }

        public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellation = default)
        {
            var vendorId = _jwtProvider.ValidateToken(token);

            if (vendorId is null)
                return Result.Failure(VendorErrors.InvalidCredentials);

            var vendor = await _userManager.FindByIdAsync(vendorId);

            if (vendor is null)
                return Result.Failure(VendorErrors.InvalidCredentials);

            var vendorRefreshToken = vendor.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

            if (vendorRefreshToken is null)
                return Result.Failure(VendorErrors.InvalidRefreshToken);

            vendorRefreshToken.RevokedOn = DateTime.UtcNow;

            await _userManager.UpdateAsync(vendor);

            return Result.Success();
        }
        public async Task<Result> RegisterAsync(RegisterationRequest request, CancellationToken cancellationToken = default) 
        {
            if (await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken))
                return Result.Failure(VendorErrors.DuplicatedEmail);

            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var vendor = new Vendor
                {
                    UserName = request.Email,
                    Email = request.Email,
                    FullName = request.FullName,
                    BusinessName = request.BusinessName,
                    BusinessType = request.BusinessType,
                    TaxNumber = request.TaxNumber,
                    // Static image paths
                    ProfilePictureUrl = "/images/vendors/OIP.jpg",
                    CoverImageUrl = "/images/vendors/StartCover.jpg",
                    IsApproved = false // Vendor starts as not approved
                };

                var result = await _userManager.CreateAsync(vendor, request.Password);
                if (!result.Succeeded)
                {
                    var error = result.Errors.First();

                    return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
                }

                await _userManager.AddToRoleAsync(vendor, "Vendor");

               
                var existingSubCategoryIds = await _context.SubCategories!
                    .Where(s => request.SubCategoryIds.Contains(s.Id))
                    .Select(s => s.Id)
                    .ToListAsync(cancellationToken);

                var invalidSubCategories = request.SubCategoryIds.Except(existingSubCategoryIds).ToList();
                if (invalidSubCategories.Any())
                    return Result.Failure(new Error("InvalidSubCategory", $"Invalid Subcategories: {string.Join(", ", invalidSubCategories)}", StatusCodes.Status400BadRequest));

                
                var vendorSubCategories = request.SubCategoryIds.Select(subCategoryId => new VendorSubCategory
                {
                    VendorId = vendor.Id,
                    SubCategoryId = subCategoryId
                }).ToList();

                await _context.VendorSubCategories!.AddRangeAsync(vendorSubCategories, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken); 
                return Result.Failure(new Error("TransactionError", ex.Message, StatusCodes.Status500InternalServerError));
            }
        }



        //public async Task<Result> LogOutAsync(string vendorId) 
        //{
        //    var user = await _userManager.FindByIdAsync(vendorId);

        //    if (user == null)
        //        return Result.Failure(new Error("Invalid User", "User not found", StatusCodes.Status404NotFound));

        //    // Revoke user's refresh tokens or perform any other cleanup
        //    var userTokens = await _context.UserTokens
        //        .Where(t => t.UserId == vendorId)
        //        .ToListAsync();

        //    _context.UserTokens.RemoveRange(userTokens);
        //    await _context.SaveChangesAsync();

        //    return Result.Success();
        //}

        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

    }
}
