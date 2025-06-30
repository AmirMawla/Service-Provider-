using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.Common;
using ServiceProvider_BLL.Dtos.ProductDto;
using ServiceProvider_BLL.Dtos.ReviewDto;
using ServiceProvider_BLL.Errors;
using ServiceProvider_BLL.Interfaces;
using ServiceProvider_DAL.Data;
using ServiceProvider_DAL.Entities;
using System.Security.Claims;
using System.Threading;

namespace SeeviceProvider_PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IUnitOfWork productRepositry,AppDbContext context) : ControllerBase
    {
        private readonly IUnitOfWork _productRepositry = productRepositry;
        private readonly AppDbContext _context = context;

        [HttpGet("")]
        [Authorize(Roles = "Admin,MobileUser")]
        [ProducesResponseType(typeof(PaginatedList<ProductResponse>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails),StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAll([FromQuery] RequestFilter request,CancellationToken cancellationToken) 
        {
            var result = await _productRepositry.Products.GetAllProductsAsync(request,cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] int id,CancellationToken cancellationToken)
        {
            var result = await _productRepositry.Products.GetProductAsync(id,cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpGet("most-requested")]
        [Authorize(Roles = "Admin,MobileUser")]
        [ProducesResponseType(typeof(List<ProductRequestCount>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMostRequested(CancellationToken cancellationToken)
        {
            var result = await _productRepositry.Products.GetMostCommonProductAsync(cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }


        [HttpGet("top5-most-requested")]
        [Authorize(Roles = "Admin,MobileUser")]
        [ProducesResponseType(typeof(List<ProductRequestCount>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Gettop5MostRequested(CancellationToken cancellationToken)
        {
            var result = await _productRepositry.Products.Gettop5MostCommonProductAsync(cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpGet("most-vendor-requested-product")]
        [Authorize(Policy ="ApprovedVendor")]
        [ProducesResponseType(typeof(IEnumerable<MostRequestedProductResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetVendorMostRequested(CancellationToken cancellationToken)
        {
            var vendorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _productRepositry.Products.GetMostRequestedProductFromAVendorAsync(vendorId!,cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpGet("most-recent")]
        [Authorize(Roles = "Admin,MobileUser")]
        [ProducesResponseType(typeof(IEnumerable<ProductResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMostRecent(CancellationToken cancellationToken)
        {
            var result = await _productRepositry.Products.GetNewProductsAsync(cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpGet("{productId}/reviews")]
        [Authorize/*(Roles ="Admin,MobileUser")*/]
        [ProducesResponseType(typeof(PaginatedList<ReviewResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetServiceReviews([FromRoute] int productId, [FromQuery] RequestFilter request, CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (User.IsInRole("Vendor"))
            {
                var vendorOwnsProduct = await _context.Products!
                    .AnyAsync(p => p.Id == productId && p.VendorId == currentUserId, cancellationToken);

                if (!vendorOwnsProduct)
                    return NotFound(ProductErrors.ProductNotFound);
            }

            var result = await productRepositry.Products.GetReviewsForSpecificServiceAsync(productId, request,cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPost("{productId}/reviews")]
        [Authorize(Roles ="MobileUser")]
        [ProducesResponseType(typeof(ReviewResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateReview([FromRoute]int productId, [FromBody] ReviewRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await productRepositry.Products.AddReviewAsync(productId,userId!,request);

            return result.IsSuccess
                ? CreatedAtAction(nameof(Get), new { id = result.Value.Id }, result.Value)
                : result.ToProblem();
        }
        
        [HttpPost("")]
        [Authorize(Policy = "AdminOrApprovedVendor")]
        [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateService([FromForm] CreateProductDto request , CancellationToken cancellationToken)

        {

            var vendorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _productRepositry.Products.AddProductAsync(vendorId!, request,cancellationToken);
            return result.IsSuccess
                ? CreatedAtAction(nameof(Get), new { id = result.Value.Id }, result.Value)
                : result.ToProblem();
        }



        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOrApprovedVendor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateService([FromRoute]int id, [FromForm] UpdateProductRequest request , CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            var result = await _productRepositry.Products.UpdateProductAsync(id, request ,currentUserId!,isAdmin, cancellationToken);
            return result.IsSuccess ? 
                NoContent():
                result.ToProblem();
        }



        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOrApprovedVendor")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteService([FromRoute]int id , CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            var result = await _productRepositry.Products.DeleteProductAsync(id,currentUserId!,isAdmin,cancellationToken);
            return result.IsSuccess 
                ? NoContent() 
                : result.ToProblem();
        }


    }
}
