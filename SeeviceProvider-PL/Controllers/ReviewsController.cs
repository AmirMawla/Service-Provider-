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
    public class ReviewsController(IUnitOfWork reviewRepository) : ControllerBase
    {
        private readonly IUnitOfWork _reviewRepository = reviewRepository;

        //[HttpGet("products/{productId}")]
        //public async Task<IActionResult> GetServiceReviews([FromRoute]int productId , CancellationToken cancellationToken) 
        //{
        //    var result = await _reviewRepository.Reviews.GetReviewsForSpecificServiceAsync(productId, cancellationToken);

        //    return result.IsSuccess ? Ok(result) : result.ToProblem();
        //}

        
        [HttpGet("{vendorId}/vendor-reviews")]
        [Authorize(Policy = "AdminOrApprovedVendor")]
        [ProducesResponseType(typeof(PaginatedList<VendorReviewsResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetVendorRatings([FromRoute] string? vendorId , [FromQuery] RequestFilter request , CancellationToken cancellationToken = default)  
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserRole == "Vendor")
            {
                // Vendors can only access their own reviews
                if (!string.IsNullOrEmpty(vendorId) && vendorId != currentUserId)
                    return Forbid();

                vendorId = currentUserId!;
            }
            else if (string.IsNullOrEmpty(vendorId))
            {
                return BadRequest("Vendor ID is required for admin users");
            }

            var result = await _reviewRepository.Reviews.GetRatingsByVendorAsync(vendorId! , request , cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpGet("user-vendor-reviews")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(PaginatedList<ReviewResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserVendorRatings([FromQuery] RequestFilter request, CancellationToken cancellationToken = default)
        {
            var result = await _reviewRepository.Reviews.GetAllRatingsFromAllUsersToAllVendorAsync(request, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPut("{reviewId}")]
        [Authorize(Roles = "MobileUser")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateReview([FromRoute]int reviewId, [FromBody] UpdateReviewRequest request) 
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _reviewRepository.Reviews.UpdateReviewAsync(reviewId,userId!,request);

               return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [HttpDelete("{reviewId}")]
        [Authorize(Roles = "MobileUser")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteReview([FromRoute] int reviewId )
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _reviewRepository.Reviews.DeleteReviewAsync(reviewId, userId!);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}
