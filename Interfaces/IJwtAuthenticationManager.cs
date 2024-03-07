using System.Security.Claims;

namespace dotnet.Models
{
    public interface IJwtAuthenticationManager
    {
        TokenResponse Authenticate(string username, string password);
        IDictionary<string, string> UsersRefreshTokens { get; set; }
        TokenResponse Authenticate(string username, Claim[] claims);
    }
}
