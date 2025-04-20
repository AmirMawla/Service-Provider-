using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ServiceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Authentication;
using ServiceProvider_BLL.Dtos.AuthenticationDto;
using ServiceProvider_BLL.Interfaces;

namespace SeeviceProvider_PL.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController(IAuthRepositry authRepositry ) : ControllerBase
    {
        private readonly IAuthRepositry _authRepositry = authRepositry;
        
        [HttpPost("")]
        public async Task<IActionResult> LogIn([FromBody] LogInRequest request , CancellationToken cancellationToken) 
        {
            var result = await _authRepositry.GetTokenAsync(request.Email, request.Password, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellation)
        {
            var result = await _authRepositry.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellation);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem() ;
        }

        [HttpPut("revoke-refresh-token")]
        public async Task<IActionResult> RevokeRefresh([FromBody] RefreshTokenRequest request, CancellationToken cancellation)
        {
            var result = await _authRepositry.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellation);

            return result.IsSuccess ? Ok() : result.ToProblem();
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterationRequest request, CancellationToken cancellationToken)
        {
            var result = await _authRepositry.RegisterAsync(request, cancellationToken);

            return result.IsSuccess? Ok(): result.ToProblem();
        }


    }
}
