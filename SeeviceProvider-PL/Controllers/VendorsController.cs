﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceProvider_BLL.Interfaces;

namespace SeeviceProvider_PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorsController(IUnitOfWork vendorRepositry) : ControllerBase
    {
        private readonly IUnitOfWork _vendorRepositry = vendorRepositry;

        [HttpGet("{categoryId}")]
        public async Task<IActionResult> GetAllVendorsInCateogry(int categoryId ,CancellationToken cancellationToken)
        {
            var result = await _vendorRepositry.Vendors.GetVendorsByCategoryIdAsync(categoryId, cancellationToken);

            return result.IsSuccess ?
                Ok(result.Value)
                : Problem(statusCode: StatusCodes.Status404NotFound, title: result.Error.code, detail: result.Error.description);
        }
    }
}