using dotnet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace dotnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly Context _db;
        private readonly IJwtAuthenticationManager jwtAuthenticationManager;
        private readonly ITokenRefresher tokenRefresher;

        public AuthenticationController(Context context, IJwtAuthenticationManager jwtAuthenticationManager, ITokenRefresher tokenRefresher)
        {
            _db = context;
            this.jwtAuthenticationManager = jwtAuthenticationManager;
            this.tokenRefresher = tokenRefresher;
        }

        [HttpPost("login")]
        public async Task<AuthenticationResponse<Login>> Login(LoginRequest loginRequest)
        {
            try
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                User usersObject = await _db.Users.FirstOrDefaultAsync(x => x.Email == loginRequest.UserName || x.Contact == loginRequest.UserName);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (usersObject == null)
                {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new AuthenticationResponse<Login>(false, "Failed: Either email or phone is incorrect.", null, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                Login loginObject = await _db.Login.FirstOrDefaultAsync(x => x.UserId == usersObject.Id);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                if (loginObject == null)
                {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                    return new AuthenticationResponse<Login>(false, "Failure: Login data doesn't exist in database.", null, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                }
                if (loginObject.Password == loginRequest.Password)
                {
                    TokenResponse token = jwtAuthenticationManager.Authenticate(loginRequest.UserName, loginRequest.Password);
                    if (!token.IsSuccess)
                    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                        return new AuthenticationResponse<Login>(false, $"{token.Message}", token, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
                    }
                    return new AuthenticationResponse<Login>(true, $"Success: Login credentials are correct.", token, loginObject);
                }
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new AuthenticationResponse<Login>(false, "Failure: Entered password is incorrect.", null, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
            catch (Exception exception)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new AuthenticationResponse<Login>(false, "Server Failure: Unable to login. Because " + exception.Message, null, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
        }

        [HttpPost("refresh")]
        public AuthenticationResponse<Login> Refresh([FromBody] RefreshCred refreshCred)
        {
            var token = tokenRefresher.Refresh(refreshCred);
            if (!token.IsSuccess)
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                return new AuthenticationResponse<Login>(false, $"{token.Message}", token, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            }
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            return new AuthenticationResponse<Login>(true, $"{token.Message}", token, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        }
    }
}
