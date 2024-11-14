using AuthService.Services;
using Common;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace AuthService.Services
{
    public class InMemoryUserStore
    {
        private readonly Dictionary<string, string> _users = new(); // Stores username/password pairs

        public bool AddUser(string username, string password)
        {
            if (_users.ContainsKey(username)) return false;
            _users.Add(username, password);
            return true;
        }

        public string GetPassword(string username)
        {
            _users.TryGetValue(username, out var password);
            return password;
        }
    }

    public class AuthService : IAuthService
    {
        private readonly InMemoryUserStore _userStore;
        private readonly IConfiguration _configuration;
        public AuthService(InMemoryUserStore userStore, IConfiguration config)
        {
            _userStore = userStore;
            _configuration = config;
        }

        public async Task<AuthResult> RegisterUserAsync(Common.RegisterRequest registerRequest)
        {
            if (_userStore.AddUser(registerRequest.Username,registerRequest.Password))
            {
                var token = GenerateJwtToken(registerRequest.Username);
                return new AuthResult { IsSuccess = true, Token = token };
            }
            return null;
        }

        public async Task<AuthResult> LoginUserAsync(Common.LoginRequest loginRequest)
        {
            var storedPassword = _userStore.GetPassword(loginRequest.Username);
            if (storedPassword == loginRequest.Password)
            {

                // Generate JWT token for the user
                var token = GenerateJwtToken(loginRequest.Username);
                return new AuthResult { IsSuccess = true, Token = token };
            }
            return null;
        }

        private string GenerateJwtToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("Jwt")["Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

