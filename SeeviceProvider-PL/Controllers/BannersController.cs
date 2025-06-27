using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceProvider_BLL.Interfaces;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Reposatories;
using ServiceProvider_BLL.Dtos.BannersDto;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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



        [HttpGet("GetDiscount")]
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
            return result.IsSuccess ? Ok(result) : result.ToProblem();

        }

    }
}
