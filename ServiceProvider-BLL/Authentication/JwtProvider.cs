using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ServiceProvider_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServiceProvider_BLL.Authentication
{
    public class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
    {
        private readonly JwtOptions _options = options.Value;
        public (string token, int expiresIn) GenerateToken(Vendor vendor,IEnumerable<string> roles)
        {
            Claim[] claims = [
                new(JwtRegisteredClaimNames.Sub,vendor.Id),
                new(JwtRegisteredClaimNames.Email,vendor.Email!),
                new(JwtRegisteredClaimNames.GivenName,vendor.FullName),
                new(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new(nameof(roles),JsonSerializer.Serialize(roles),JsonClaimValueTypes.JsonArray),
                new ("IsApproved", vendor.IsApproved.ToString())
            ];

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));

            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);


            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_options.ExpireyMinutes),
                signingCredentials: signingCredentials
                );

            return (token: new JwtSecurityTokenHandler().WriteToken(token), expiresIn: _options.ExpireyMinutes * 60);
        }

        public string? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    IssuerSigningKey = symmetricSecurityKey,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero

                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                return jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
            }
            catch
            {
                return null;
            }
        }
    }
}
