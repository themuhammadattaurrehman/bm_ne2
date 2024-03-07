using dotnet.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace dotnet.Authentication
{
    public class TokenRefresher : ITokenRefresher
    {
        private readonly byte[] key;
        private readonly IJwtAuthenticationManager jWTAuthenticationManager;
        IDictionary<string, string> UsersRefreshTokens { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public TokenRefresher(byte[] key, IJwtAuthenticationManager jWTAuthenticationManager)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            this.key = key;
            this.jWTAuthenticationManager = jWTAuthenticationManager;
        }

        public TokenResponse Refresh(RefreshCred refreshCred)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken validatedToken;
                var pricipal = tokenHandler.ValidateToken(refreshCred.JwtToken,
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = false,
                    }, out validatedToken);
                var jwtToken = validatedToken as JwtSecurityToken;
                if (jwtToken == null || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return new TokenResponse
                    {
                        IsSuccess = false,
                        Message = "Failure: Invalid token passed. Therefore, unable to refresh token.",
                    };
                }

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var userName = pricipal.Identity.Name;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.
                if (refreshCred.RefreshToken != jWTAuthenticationManager.UsersRefreshTokens[userName])
                {
                    return new TokenResponse
                    {
                        IsSuccess = false,
                        Message = "Failure: Invalid token passed. Therefore, unable to refresh token.",
                    };
                }
#pragma warning restore CS8604 // Possible null reference argument.

                return jWTAuthenticationManager.Authenticate(userName, pricipal.Claims.ToArray());
            }
            catch (Exception exception)
            {
                return new TokenResponse
                {
                    IsSuccess = false,
                    Message = $"Server Failure: {exception.Message}",
                };
            }
        }

    }
}
