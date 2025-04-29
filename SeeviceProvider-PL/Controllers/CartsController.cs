using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.CartProductDto;
using ServiceProvider_BLL.Interfaces;
using ServiceProvider_DAL.Entities;
using System.Security.Claims;

namespace SeeviceProvider_PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "MobileUser")]
    public class CartsController(IUnitOfWork CartRepositry) : ControllerBase
    {
        private readonly IUnitOfWork _cartRepositry = CartRepositry;

        [HttpGet("")]
        public async Task<IActionResult> GetCartDetalis( CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _cartRepositry.Carts.GetCart(userId!, cancellationToken);

            return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
        }

        [HttpPost("items")]
        public async Task<IActionResult> AddToCart([FromBody] CartProductRequest request , CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _cartRepositry.Carts.AddToCartAsync(userId!,request ,cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        
        [HttpPut("items")]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemRequest request , CancellationToken cancellationToken)
        {
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _cartRepositry.Carts.UpdateCartItemAsync(userId! ,request , cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }
    }
}
