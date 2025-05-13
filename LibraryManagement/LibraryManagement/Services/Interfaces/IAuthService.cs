

using LibraryManagement.DTOs.Auth;

namespace LibraryManagement.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request);
    Task LogoutAsync(string username);
}
