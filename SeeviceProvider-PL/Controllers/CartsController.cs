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
    [Authorize(Roles = "MobileUser,Admin")]
    public class CartsController(IUnitOfWork CartRepositry) : ControllerBase
    {
        private readonly IUnitOfWork _cartRepositry = CartRepositry;

        [HttpGet("")]
        [ProducesResponseType(typeof(CartResponse),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCartDetalis( CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _cartRepositry.Carts.GetCart(userId!, cancellationToken);

            return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
        }

        [HttpPost("items")]
        [ProducesResponseType(typeof(CartProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddToCart([FromBody] CartProductRequest request , CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _cartRepositry.Carts.AddToCartAsync(userId!,request ,cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        
        [HttpPut("items")]
        [ProducesResponseType(typeof(CartProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemRequest request , CancellationToken cancellationToken)
        {
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _cartRepositry.Carts.UpdateCartItemAsync(userId! ,request , cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }


        [HttpDelete("{ProductId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSpecificProduct( [FromRoute]int ProductId,CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _cartRepositry.Carts.DeleteSpecificProduct(userId!, ProductId, cancellationToken);
            return result.IsSuccess
                  ? NoContent()
                  : result.ToProblem();
        }


        [HttpDelete("All")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAllProducts( CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _cartRepositry.Carts.DeleteallProducts(userId!, cancellationToken);

            return result.IsSuccess
                  ? NoContent()
                  : result.ToProblem();
        }
    }
}
