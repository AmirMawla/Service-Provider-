using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Dtos.AuthenticationDto
{
    public record AuthResponse(
        string Id,
        string? Email,
        string FullName,
        string? BusinessName,
        string BusinessType,
        string Token,
        int ExpiresIn,
        string RefreshToken,
        DateTime RefreshTokenExpiration
    );
}
