using ServiceProvider_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Authentication
{
    public interface IJwtProvider
    {
        (string token, int expiresIn) GenerateToken(Vendor vendor , IEnumerable<string> roles);
        string? ValidateToken(string token);
    }
}
