using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.CategoryDto;
using ServiceProvider_BLL.Interfaces;
using ServiceProvider_DAL.Entities;

namespace SeeviceProvider_PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController(IUnitOfWork categoryRepositry) : ControllerBase
    {
        private readonly IUnitOfWork _categoryRepositry = categoryRepositry;

        [HttpGet("")]
        //[Authorize(Roles = "MobileUser")]
        public async Task<IActionResult> GetAllCategories(CancellationToken cancellationToken) 
        {
            var result = await _categoryRepositry.Categories.GetCategoriesAsync(cancellationToken);

            return result.IsSuccess ?
            Ok(result.Value)
            :result.ToProblem();

        }

        [HttpGet("{categoryId}/providers")]
        public async Task<IActionResult> GetProvidersByCategory([FromRoute] int categoryId , CancellationToken cancellationToken)
        {
            var result = await _categoryRepositry.Categories.GetProvidersByCategoryAsync(categoryId , cancellationToken);
            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpGet("{categoryId}/subCategories")]
        public async Task<IActionResult> GetSubCategoryByCategory([FromRoute] int categoryId, CancellationToken cancellationToken)
        {
            var result = await _categoryRepositry.Categories.GetSubCategoryByCategoryAsync(categoryId, cancellationToken);
            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpPost("")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> AddCategory([FromBody] CategoryRequest request, CancellationToken cancellationToken)
        {
            var result = await _categoryRepositry.Categories.AddCategoryAsync(request, cancellationToken);

            return result.IsSuccess
                ? CreatedAtAction(nameof(GetAllCategories), new { id = result.Value.Id }, result.Value)
                : result.ToProblem();
        }

        [HttpPost("{categoryId}/subcategories")]
        [Authorize(Policy = "AdminOrApprovedVendor")]
        public async Task<IActionResult> CreateSubCategory( [FromRoute] int categoryId,[FromBody] SubCategoryRequest request , CancellationToken cancellationToken)
        {

            var result = await _categoryRepositry.Categories.AddSubCategoryAsync(categoryId,request,cancellationToken);

            return result.IsSuccess
                ? CreatedAtAction(nameof(GetAllCategories), new { id = result.Value.Id }, result.Value)
                : result.ToProblem();
        }


        
        [HttpDelete("subcategories/{subCategoryId}")]
        [Authorize(Policy = "AdminOrApprovedVendor")]
        public async Task<IActionResult> DeleteSubCategory([FromRoute] int subCategoryId, CancellationToken cancellationToken)
        {

            var result = await _categoryRepositry.Categories.DeleteSubCategoryAsync(subCategoryId ,cancellationToken);

            return result.IsSuccess
                ? Ok()
                : result.ToProblem();
        }
    }
}
