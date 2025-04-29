using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.Common;
using ServiceProvider_BLL.Dtos.ProductDto;
using ServiceProvider_BLL.Dtos.ReviewDto;
using ServiceProvider_BLL.Interfaces;
using ServiceProvider_DAL.Entities;
using System.Security.Claims;
using System.Threading;

namespace SeeviceProvider_PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IUnitOfWork productRepositry) : ControllerBase
    {
        private readonly IUnitOfWork _productRepositry = productRepositry;

        [HttpGet("")]
        public async Task<IActionResult> GetAll([FromQuery] RequestFilter request,CancellationToken cancellationToken) 
        {
            var result = await _productRepositry.Products.GetAllProductsAsync(request,cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id,CancellationToken cancellationToken)
        {
            var result = await _productRepositry.Products.GetProductAsync(id,cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpGet("most-requested")]
        public async Task<IActionResult> GetMostRequested(CancellationToken cancellationToken)
        {
            var result = await _productRepositry.Products.GetMostCommonProductAsync(cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpGet("most-recent")]
        public async Task<IActionResult> GetMostRecent(CancellationToken cancellationToken)
        {
            var result = await _productRepositry.Products.GetNewProductsAsync(cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpGet("{productId}/reviews")]
        public async Task<IActionResult> GetServiceReviews([FromRoute] int productId, CancellationToken cancellationToken)
        {
            var result = await productRepositry.Products.GetReviewsForSpecificServiceAsync(productId, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPost("{productId}/reviews")]
        public async Task<IActionResult> CreateReview([FromRoute]int productId, [FromBody] ReviewRequest request)
        {
            var result = await productRepositry.Products.AddReviewAsync(productId,request);

            return result.IsSuccess
                ? CreatedAtAction(nameof(Get), new { id = result.Value.Id }, result.Value)
                : result.ToProblem();
        }
        
        [HttpPost("")]
        [Authorize(Policy = "AdminOrApprovedVendor")]
        public async Task<IActionResult> CreateService([FromBody] ProductRequest request , CancellationToken cancellationToken)
        {

            var vendorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _productRepositry.Products.AddProductAsync(vendorId!, request,cancellationToken);
            return result.IsSuccess
                ? CreatedAtAction(nameof(Get), new { id = result.Value.Id }, result.Value)
                : result.ToProblem();
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOrApprovedVendor")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] UpdateProductRequest request , CancellationToken cancellationToken)
        {

            var vendorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _productRepositry.Products.UpdateProductAsync(id, request, vendorId!,cancellationToken);
            return result.IsSuccess ? 
                NoContent():
                result.ToProblem();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOrApprovedVendor")]
        public async Task<IActionResult> DeleteService(int id , CancellationToken cancellationToken)
        {
            var vendorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _productRepositry.Products.DeleteProductAsync(id, vendorId!,cancellationToken);
            return result.IsSuccess 
                ? NoContent() 
                : result.ToProblem();
        }
    }
}
