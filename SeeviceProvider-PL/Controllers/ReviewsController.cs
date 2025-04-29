using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.Common;
using ServiceProvider_BLL.Dtos.ReviewDto;
using ServiceProvider_BLL.Interfaces;
using ServiceProvider_DAL.Entities;
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

        [Authorize]
        [HttpGet("{vendorId}/vendor-reviews")]
        public async Task<IActionResult> GetVendorRatings([FromRoute] string vendorId , [FromQuery] RequestFilter request , CancellationToken cancellationToken = default)  
        {
            var result = await _reviewRepository.Reviews.GetRatingsByVendorAsync(vendorId , request , cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpGet("user-vendor-reviews")]
        public async Task<IActionResult> GetUserVendorRatings([FromQuery] RequestFilter request, CancellationToken cancellationToken = default)
        {
            var result = await _reviewRepository.Reviews.GetAllRatingsFromAllUsersToAllVendorAsync(request, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPut("{reviewId}")]
        public async Task<IActionResult> UpdateReview([FromRoute]int reviewId, [FromBody] UpdateReviewRequest request) 
        {
            var result = await _reviewRepository.Reviews.UpdateReviewAsync(reviewId,request);

               return result.IsSuccess ? Ok() : result.ToProblem();
        }

        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteReview([FromRoute] int reviewId , [FromBody]string userId)
        {
            var result = await _reviewRepository.Reviews.DeleteReviewAsync(reviewId, userId);

            return result.IsSuccess ? Ok() : result.ToProblem();
        }
    }
}
