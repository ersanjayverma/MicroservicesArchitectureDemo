using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.Data;
using Common;
using AuthService.Services;
namespace AuthService.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="registerRequest">The registration details of the new user.</param>
        /// <returns>Success or failure response.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Common.RegisterRequest registerRequest)
        {
            var result = await _authService.RegisterUserAsync(registerRequest);
            if (result.IsSuccess)
            {
                return Ok(new { message = "Registration successful." });
            }

            return BadRequest(new { message = result.ErrorMessage });
        }

        /// <summary>
        /// Logs in a user and returns a JWT token.
        /// </summary>
        /// <param name="loginRequest">The login credentials of the user.</param>
        /// <returns>A JWT token if login is successful.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Common.LoginRequest loginRequest)
        {
            var result = await _authService.LoginUserAsync(loginRequest);
            if (result.IsSuccess)
            {
                return Ok(new { token = result.Token });
            }

            return Unauthorized(new { message = "Invalid credentials." });
        }

        /// <summary>
        /// Verifies the validity of the provided JWT token.
        /// </summary>
        /// <returns>User information if the token is valid.</returns>
        [HttpGet("verify")]
        [Authorize]
        public IActionResult Verify()
        {
            var userId = User.Identity.Name; // Extract the user ID from the JWT claims
            return Ok(new { message = "Token is valid.", userId });
        }
    }
}
