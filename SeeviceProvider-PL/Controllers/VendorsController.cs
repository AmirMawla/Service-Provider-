﻿using Government.Contracts.AccountProfile.cs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.CategoryDto;
using ServiceProvider_BLL.Dtos.Common;
using ServiceProvider_BLL.Dtos.PaymentDto;
using ServiceProvider_BLL.Dtos.ProductDto;
using ServiceProvider_BLL.Dtos.VendorDto;
using ServiceProvider_BLL.Errors;
using ServiceProvider_BLL.Interfaces;
using ServiceProvider_BLL.Reposatories;
using ServiceProvider_DAL.Data;
using ServiceProvider_DAL.Entities;
using System.Security.Claims;
using System.Threading;

namespace SeeviceProvider_PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorsController(IUnitOfWork vendorRepositry,AppDbContext appDbContext) : ControllerBase
    {
        private readonly IUnitOfWork _vendorRepositry = vendorRepositry;
        private readonly AppDbContext _appDbContext = appDbContext;

        [HttpGet("")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(PaginatedList<VendorResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProviders([FromQuery] RequestFilter request, CancellationToken cancellationToken)
        {
            var result = await _vendorRepositry.Vendors.GetAllProviders(request,cancellationToken);
            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpGet("for-mobile")]
        [Authorize(Roles = "MobileUser")]
        [ProducesResponseType(typeof(PaginatedList<VendorResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProvidersForMobile([FromQuery] RequestFilter request, CancellationToken cancellationToken)
        {
            var result = await _vendorRepositry.Vendors.GetAllProvidersForMobile(request, cancellationToken);
            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpGet("vendors-rating")]
        [Authorize(Policy ="AdminOrApprovedVendor")]
        [ProducesResponseType(typeof(PaginatedList<VendorRatingResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProvidersRatings(string? vendorId,[FromQuery] RequestFilter request, CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserRole == "Vendor")
            {
                // Vendors can only access their own reviews
                if (!string.IsNullOrEmpty(vendorId) && vendorId != currentUserId)
                    return Forbid();

                vendorId = currentUserId;
            }
            else if (string.IsNullOrEmpty(vendorId))
            {
                return BadRequest("Vendor ID is required for admin users");
            }

            var result = await _vendorRepositry.Vendors.GetVendorsRatings(vendorId!,request, cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpGet("vendors-top-selling-products")]
        [Authorize(Policy = "AdminOrApprovedVendor")]
        [ProducesResponseType(typeof(IEnumerable<TopFiveProductsWithVendorResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTopFiveProductsWithVendor(string? vendorId, CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserRole == "Vendor")
            {
                // Vendors can only access their own reviews
                if (!string.IsNullOrEmpty(vendorId) && vendorId != currentUserId)
                    return Forbid();

                vendorId = currentUserId;
            }
            else if (string.IsNullOrEmpty(vendorId))
            {
                return BadRequest("Vendor ID is required for admin users");
            }

            var result = await _vendorRepositry.Vendors.GetTopFiveProductsWithVendor(vendorId!, cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }


        [HttpGet("{providerId}")]
        [Authorize(Roles = "Admin,MobileUser")]
        [ProducesResponseType(typeof(VendorResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProviderDetalis([FromRoute] string providerId, CancellationToken cancellationToken)
        {
            var result = await _vendorRepositry.Vendors.GetProviderDetails(providerId, cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpGet("my-profile")]
        [Authorize(Policy = "ApprovedVendor")]
        [ProducesResponseType(typeof(VendorResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProviderDetalis(CancellationToken cancellationToken)
        {
            var providerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var result = await _vendorRepositry.Vendors.GetProviderProfile(providerId, cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpGet("vendor-transactions")]
        [Authorize(Policy = "ApprovedVendor")]
        [ProducesResponseType(typeof(PaginatedList<VendorTransactionResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProviderTransactions([FromQuery]RequestFilter request,CancellationToken cancellationToken)
        {
            var providerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var result = await _vendorRepositry.Payments.GetVendorTransactionsAsync(request,providerId, cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpGet("top-5-vendors")]
        [Authorize(Roles = "MobileUser")]
        [ProducesResponseType(typeof(IEnumerable<TopVendorResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTopVendors(CancellationToken cancellationToken =default)
        {
            var result = await _vendorRepositry.Vendors.GetTopVendorsByOrders();

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpGet("BusinessTypes")]
        //[Authorize(Policy = "AdminOrApprovedVendor")]
        [ProducesResponseType(typeof(VendorBusinessTypeRespons), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProvidersBusinessTypes(CancellationToken cancellationToken)
        {
            var result = await _vendorRepositry.Vendors.GetAllVendorsBusinessTypes(cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }


        [HttpGet("vendor-dashboard")]
        [Authorize(Policy = "ApprovedVendor")]
        [ProducesResponseType(typeof(VendorDashboardResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDashboard(CancellationToken cancellationToken = default)
        {
            var result = await _vendorRepositry.Vendors.GetVendorDashboard(cancellationToken);
            return Ok(result);
        }

        [HttpGet("vendor-Revenue-ByPaymentMethod")]
        [Authorize(Policy = "ApprovedVendor")]
        [ProducesResponseType(typeof(VendorRevenueByPaymentMethod), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetVendorRevenueByPaymentMethod(CancellationToken cancellationToken = default)
        {
            var vendorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var result = await _vendorRepositry.Payments.GetVendorRevenueByPaymentMethod(vendorId,cancellationToken);
            return Ok(result.Value);
        }

        [HttpGet("menu")]
        [HttpGet("{providerId}/menu")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<ProductsOfVendorDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProviderMenu(string? providerId, CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserRole == "Vendor")
            {
                // Vendors can only access their own reviews
                if (!string.IsNullOrEmpty(providerId) && providerId != currentUserId)
                    return Forbid();

                providerId = currentUserId;
            }
            else if (string.IsNullOrEmpty(providerId))
            {
                return BadRequest("Vendor ID is required for admin and mobile users");
            }

            var result = await _vendorRepositry.Vendors.GetProviderMenuAsync(providerId, cancellationToken);
            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }


        [HttpPut("update-profile")]
        [Authorize(Policy = "ApprovedVendor")]
        public async Task<IActionResult> UpdateVendor( [FromForm] UpdateVendorRequest vendorDto, CancellationToken cancellationToken)
        {
            string vendorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var result = await _vendorRepositry.Vendors.UpdateVendorAsync(vendorId, vendorDto, cancellationToken);
            return result.IsSuccess ?
                NoContent() :
                result.ToProblem();
        }

        [HttpPut("change-password")]
        [Authorize(Policy = "AdminOrApprovedVendor")]
        public async Task<IActionResult> ChangeVendorPassword([FromBody] ChangeVendorPasswordRequest request)
        {
            var vendorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _vendorRepositry.Vendors.ChangeVendorPasswordAsync(vendorId!, request);
            return result.IsSuccess
                 ? Ok("Password updated successfully")
                 : result.ToProblem();

        }

        [HttpDelete("{providerId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVendor([FromRoute] string providerId, CancellationToken cancellationToken)
        {
            var result = await _vendorRepositry.Vendors.DeleteVendorAsync(providerId, cancellationToken);
            return result.IsSuccess
                ? NoContent()
                : result.ToProblem();
        }

        [HttpGet("pending-vendors")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(IEnumerable<VendorResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPendingVendors(CancellationToken cancellationToken)
        {
            var result = await _vendorRepositry.Vendors.GetPendingVendorsAsync(cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPost("approve-vendor/{vendorId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> ApproveVendor([FromRoute] string vendorId)
        {
            var result = await _vendorRepositry.Vendors.ApproveVendorAsync(vendorId);

            return result.IsSuccess ? Ok() : result.ToProblem();
        }

        //[HttpPut("{id}/update-images")]
        //public async Task<IActionResult> UpdateVendorImages(string id, [FromBody] VendorImageUpdateDto dto)
        //{
        //    var vendor = await _appDbContext.Users.FindAsync(id);
        //    if (vendor == null) return NotFound();

        //    vendor.ProfilePictureUrl = dto.ProfilePictureUrl;
        //    vendor.CoverImageUrl = dto.CoverImageUrl;

        //    await _appDbContext.SaveChangesAsync();
        //    return Ok(vendor);
        //}

        [HttpPost("deactivate-vendor/{vendorId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeactivateVendor([FromRoute] string vendorId)
        {
            var result = await _vendorRepositry.Vendors.DeactivateVendorAsync(vendorId);

            return result.IsSuccess ? Ok() : result.ToProblem();
        }

        
        [HttpPost("forgot-password")]
        //[Authorize(Policy = "AdminOrApprovedVendor")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto, CancellationToken ct)
        {
            await _vendorRepositry.Vendors.GenerateAndSendAsync(dto.Email, ct);
            return NoContent();
        }


        // 2) يتحقق من OTP
        [HttpPost("verify-otp")]
        //[Authorize(Policy = "AdminOrApprovedVendor")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpDto dto, CancellationToken ct)
        {

            var result = await _vendorRepositry.Vendors.VerifyAsync(dto.Email, dto.Otp, ct);

            if (result.IsSuccess)
                return Ok(result.Value);


            return result.ToProblem();// for user not found

        }

        // 3) يغيّر كلمة المرور
        [HttpPost("reset-password")]
        //[Authorize(Policy = "AdminOrApprovedVendor")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto, CancellationToken ct)
        {
            var result = await _vendorRepositry.Vendors.ResetUserPassword(dto.Email, dto.ResetToken, dto.NewPassword, ct);

            return (result.IsSuccess)
                   ? NoContent()
                   : result.ToProblem();
        }



    }
}

