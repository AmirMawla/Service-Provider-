using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceProvider_BLL.Interfaces;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Reposatories;
using ServiceProvider_BLL.Dtos.BannersDto;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using ServiceProvider_DAL.Entities;
using System.Reflection;

namespace SeeviceProvider_PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannersController(IUnitOfWork BannersRepository) : ControllerBase
    {
        private readonly IUnitOfWork bannersRepository = BannersRepository;
        [HttpGet("top-Banners")]
        [ProducesResponseType(typeof(IEnumerable<BannersResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTopBanners(CancellationToken cancellationToken)
        {
            var result = await bannersRepository.Banners.GetTopBannersAsync(cancellationToken);

            return result.IsSuccess
           ? Ok(result.Value)
           : result.ToProblem();
        }



        [HttpGet("vendor/banners")]
        [Authorize(Policy = "ApprovedVendor")]
        [ProducesResponseType(typeof(IEnumerable<BannerResponse2>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBannersForCurrentVendor(CancellationToken cancellationToken = default)
        {
            var vendorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var banners = await bannersRepository.Banners.GetVendorBannersAsync(vendorId!, cancellationToken);

            return banners.IsSuccess
           ? Ok(banners.Value)
           : banners.ToProblem();
        }



        [HttpPut("update")]
        [Authorize(Policy = "ApprovedVendor")]
        [ProducesResponseType(typeof(BannerResponse2), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateBanner([FromForm] BannerRequest2 request, CancellationToken cancellationToken)
        {
            var vendorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var banner = await bannersRepository.Banners.UpdateBannerAsync(vendorId!, request, cancellationToken);


            return banner.IsSuccess ? Ok(banner.Value) : banner.ToProblem();
        }




        [HttpDelete("{productid}")]
        [Authorize(Policy = "ApprovedVendor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteBanner([FromRoute] int productid, [FromQuery] decimal DiscountPercentage , CancellationToken cancellationToken)
        {
            var vendorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result =await bannersRepository.Banners.DeleteBannerAsync(vendorId!, productid, DiscountPercentage, cancellationToken);
            return NoContent();
        }




        [HttpPost("vendor/banners")]
        [Authorize(Policy = "ApprovedVendor")]
        [ProducesResponseType(typeof(BannerResponse2), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddBanner([FromForm] BannerRequest2 request, CancellationToken cancellationToken)
        {
            var vendorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var banner = await bannersRepository.Banners.AddBannerAsync(vendorId!, request, cancellationToken);

            return CreatedAtAction(nameof(GetBannersForCurrentVendor), new { }, banner
            );
        }




        [HttpGet("GetDiscount")]
        [Authorize(Roles = "Admin,MobileUser")]
        [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCartDiscountByCode(string? UserId,[FromQuery]string discountCode, CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserRole == "MobileUser")
            {
                // users can only access their own orders
                if (!string.IsNullOrEmpty(UserId) && UserId != currentUserId)
                    return Forbid();

                UserId = currentUserId;
            }
            else if (string.IsNullOrEmpty(UserId))
            {
                return BadRequest("User ID is required for admin users");
            }

            var result = await bannersRepository.Banners.GetCartDiscountByCodeAsync(discountCode, UserId!, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();

        }



    }
}
