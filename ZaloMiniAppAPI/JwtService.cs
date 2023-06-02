using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ZaloMiniAppAPI
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtService(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public string GenerateJwtToken(int customerId, bool isAdmin)
        {
            var claims = new List<Claim>
    {
        new Claim("CustomerId", customerId.ToString())
    };

            if (isAdmin)
            {
                claims.Add(new Claim("isAdmin", "true"));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddDays(_jwtSettings.ExpiryInDays);

            var token = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims,
                expires: expires,
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        public bool ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                // Validate the token and retrieve the claims
                SecurityToken validatedToken;
                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

                // Retrieve the value of the "isAdmin" claim
                var isAdminClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "isAdmin");
                if (isAdminClaim != null && bool.TryParse(isAdminClaim.Value, out bool isAdmin))
                {
                    // Perform any necessary actions for admin users
                    if (isAdmin)
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                // Token validation failed
                return false;
            }

            // The user is not an admin or the isAdmin claim is not present
            // Add your logic here for non-admin users
            return false;
        }
    }
}
