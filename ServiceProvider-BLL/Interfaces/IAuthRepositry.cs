using SeeviceProvider_BLL.Abstractions;
using ServiceProvider_BLL.Dtos.AuthenticationDto;
using ServiceProvider_BLL.Dtos.VendorDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Interfaces
{
    public interface IAuthRepositry
    {
        Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellation);
        Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellation = default);
        Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellation = default);
        Task<Result> RegisterAsync(RegisterationRequest request, CancellationToken cancellationToken = default);

    }
}
