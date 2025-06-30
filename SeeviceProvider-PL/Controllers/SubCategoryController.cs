using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.CategoryDto;
using ServiceProvider_BLL.Dtos.Common;
using ServiceProvider_BLL.Dtos.ProductDto;
using ServiceProvider_BLL.Dtos.ReviewDto;
using ServiceProvider_BLL.Interfaces;
using ServiceProvider_BLL.Reposatories;
using ServiceProvider_BLL.Errors;
using Azure.Core;
using System.Security.Claims;

using ServiceProvider_DAL.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace SeeviceProvider_PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoryController(IUnitOfWork SubCategoryRepositry) : ControllerBase
    {
        private readonly IUnitOfWork _subcategoryRepositry = SubCategoryRepositry;

        [HttpGet("")]
        [Authorize(Roles = "Admin,MobileUser")]
        [ProducesResponseType(typeof(IEnumerable<SubCategoryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllSubCategories(CancellationToken cancellationToken)
        {
            var result = await _subcategoryRepositry.SubCategories.GetSubCategoriesAsync(cancellationToken);

            return result.IsSuccess ?
            Ok(result.Value)
            : result.ToProblem();

        }



        [HttpGet("{providerId}/SubCategories")]
        [HttpGet("SubCategories")]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<SubCategoryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSubCategoriesByVendor(string? providerId, CancellationToken cancellationToken)
        {

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserRole == "Vendor")
            {
                // Vendors can only access their own reviews
                if (!string.IsNullOrEmpty(providerId) && providerId != currentUserId)
                    return Forbid();

                providerId = currentUserId!;
            }
            else if (string.IsNullOrEmpty(providerId))
            {
                return BadRequest("Vendor ID is required for admin and mobile users");
            }

            var result = await _subcategoryRepositry.SubCategories.GetSubCategoriesUnderVendorAsync(providerId, cancellationToken);

            return result.IsSuccess ? Ok(result.Value):result.ToProblem();
        }

        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        //public async Task<IActionResult> GetSubCategoriesUnderVendor([FromRoute] string providerId , CancellationToken cancellationToken )
        //{


        //    var result = await _subcategoryRepositry.SubCategories.GetSubCategoriesUnderVendorAsync(providerId, cancellationToken);

        //    return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        //}


        //[HttpGet("MySubCategories")]
        //[Authorize(Policy = "ApprovedVendor")]
        //[ProducesResponseType(typeof(IEnumerable<SubCategoryResponse>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
        //public async Task<IActionResult> GetMySubcategories(CancellationToken cancellationToken)
        //{
        //    var providerId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        //    var result = await _subcategoryRepositry.SubCategories.GetSubCategoriesUnderVendorAsync(providerId, cancellationToken);

        //    return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        //}



        [HttpGet("{subCategoryId}/products")]
        [Authorize(Roles = "Admin,MobileUser")]
        [ProducesResponseType(typeof(PaginatedList<ProductResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllProductsUnderSubCategory([FromRoute] int subCategoryId, [FromQuery] RequestFilter request, CancellationToken cancellationToken)
        {
            var result = await _subcategoryRepositry.Products.GetAllProductsUnderSubcategoryAsync(subCategoryId, request, cancellationToken);

            return result.IsSuccess ?
            Ok(result.Value)
            : result.ToProblem();

        }


        [HttpGet("{providerId}/{subCategoryId}/Products")]
        [Authorize(Roles = "Admin,MobileUser")]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProductsByVendorAndSubCategory([FromRoute] string providerId, [FromRoute] int subCategoryId, CancellationToken cancellationToken)
        {
            var result = await _subcategoryRepositry.Products.GetAllProductsUnderSubcategoryandVendorAsync(providerId, subCategoryId, cancellationToken);

            return result.IsSuccess ?
            Ok(result.Value)
            : result.ToProblem();

        }

    }
} 
