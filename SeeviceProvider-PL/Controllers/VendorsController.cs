using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.Common;
using ServiceProvider_BLL.Dtos.VendorDto;
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
        public async Task<IActionResult> GetProviders([FromQuery] RequestFilter request, CancellationToken cancellationToken)
        {
            var result = await _vendorRepositry.Vendors.GetAllProviders(request,cancellationToken);
            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpGet("vendors-rating")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetProvidersRatings([FromQuery] RequestFilter request, CancellationToken cancellationToken)
        {
            var result = await _vendorRepositry.Vendors.GetVendorsRatings(request, cancellationToken);
            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }


        [HttpGet("{providerId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetProviderDetalis([FromRoute] string providerId, CancellationToken cancellationToken)
        {
            var result = await _vendorRepositry.Vendors.GetProviderDetails(providerId, cancellationToken);
            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }


        [HttpGet("{providerId}/menu")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetProviderMenu([FromRoute] string providerId, CancellationToken cancellationToken)
        {
            var result = await _vendorRepositry.Vendors.GetProviderMenuAsync(providerId, cancellationToken);
            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVendor(string id, [FromBody] UpdateVendorResponse vendorDto, CancellationToken cancellationToken)
        {

            var result = await _vendorRepositry.Vendors.UpdateVendorAsync(id, vendorDto, cancellationToken);
            return result.IsSuccess
                 ? Ok(result.Value)
                 : result.ToProblem();
        }

        [HttpPut("change-password")]
        //[Authorize(Roles = "Admin")]
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
        public async Task<IActionResult> DeleteVendor([FromRoute] string providerId, CancellationToken cancellationToken)
        {
            var result = await _vendorRepositry.Vendors.DeleteVendorAsync(providerId, cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ToProblem();
        }

        [HttpGet("pending-vendors")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingVendors(CancellationToken cancellationToken)
        {
            var result = await _vendorRepositry.Vendors.GetPendingVendorsAsync(cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPost("approve-vendor/{vendorId}")]
        [Authorize(Roles = "Admin")]
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
        public async Task<IActionResult> DeactivateVendor([FromRoute] string vendorId)
        {
            var result = await _vendorRepositry.Vendors.DeactivateVendorAsync(vendorId);

            return result.IsSuccess ? Ok() : result.ToProblem();
        }



    }
}

