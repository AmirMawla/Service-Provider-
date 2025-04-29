using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.Common;
using ServiceProvider_BLL.Interfaces;

namespace SeeviceProvider_PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController(IAnalyticsRepositry analyticsRepositry,IUnitOfWork generalRepository) : ControllerBase
    {
        private readonly IAnalyticsRepositry _analyticsRepositry = analyticsRepositry;
        private readonly IUnitOfWork _generalRepository = generalRepository;

        [HttpGet("today-stats")]
        public async Task<IActionResult> GetTodayStats(CancellationToken cancellationToken = default) 
        {
            var result = await _analyticsRepositry.GetTodaysStatsAsync(cancellationToken);

            return Ok(result.Value);
        }

        [HttpGet("top-vendors")]
        public async Task<IActionResult> GetTopVendors()
        {
            var result = await _analyticsRepositry.GetTopVendorsAsync();

            return Ok(result.Value);
        }

        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllMobileUsers([FromQuery] RequestFilter request , CancellationToken cancellationToken = default)
        {
            var result = await _generalRepository.ApplicationUsers.GetAllMobileUsers(request,cancellationToken);

            return result.IsSuccess? Ok(result.Value): result.ToProblem();
        }

        [HttpGet("all-users-count")]
        
        public async Task<IActionResult> GetAllMobileUsersCount( CancellationToken cancellationToken = default)
        {
            var result = await _generalRepository.ApplicationUsers.GetTotalUsersCountAsync(cancellationToken);

            return result.IsSuccess ? Ok(new { TotalApplicationUsers = result.Value }) : result.ToProblem();
        }

        [HttpGet("all-transactions")]
        public async Task<IActionResult> GetTransactions ([FromQuery] RequestFilter request, CancellationToken cancellationToken = default)
        {
            var result = await _generalRepository.Payments.GetAllTransactions(request, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpGet("all-transactions-count")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTransactionsCount( CancellationToken cancellationToken = default)
        {
            var result = await _generalRepository.Payments.GetTotalTransactionsCountAsync(cancellationToken);

            return result.IsSuccess ? Ok(new { TotalTransactionsCount = result.Value }) : result.ToProblem();
        }

        [HttpGet("users/{userId}/all-transactions")]
        public async Task<IActionResult> GetUserTransactions([FromRoute] string userId,[FromQuery] RequestFilter request, CancellationToken cancellationToken = default)
        {
            var result = await _generalRepository.Payments.GetUserTransactions(userId,request, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }



        [HttpGet("project-summary")]
        public async Task<IActionResult> GetProjectSummary()
        {
            var result = await _analyticsRepositry.GetOverallStatisticsAsync();

            return Ok(result.Value);
        }



        //[Authorize]
        //[HttpGet("debug-claims")]
        //public IActionResult DebugClaims()
        //{
        //    var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        //    return Ok(claims);
        //}
    }
}
