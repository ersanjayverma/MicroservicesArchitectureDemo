using Common;

namespace AuthService.Services
{
    public interface IAuthService
    {

        Task<AuthResult> RegisterUserAsync(RegisterRequest registerRequest);
        Task<AuthResult> LoginUserAsync(LoginRequest loginRequest);

    }
}