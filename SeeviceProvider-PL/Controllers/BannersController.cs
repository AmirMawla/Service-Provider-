using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceProvider_BLL.Interfaces;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Reposatories;
using ServiceProvider_BLL.Dtos.BannersDto;

namespace SeeviceProvider_PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannersController(IUnitOfWork BannersRepository) : ControllerBase
    {
        private readonly IUnitOfWork bannersRepository = BannersRepository;
        [HttpGet("top-Banners")]
        [ProducesResponseType(typeof(IEnumerable<BannersResponse>),StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTopBanners(CancellationToken cancellationToken)
        {
            var result = await bannersRepository.Banners.GetTopBannersAsync(cancellationToken);

            return result.IsSuccess
           ? Ok(result.Value)
           : result.ToProblem();
        }
    }
}
