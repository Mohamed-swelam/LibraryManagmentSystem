using Core.DTOs;
using Core.DTOs.AuthDTO;
using Core.Helpers;

namespace Core.Interfaces
{
    public interface IAuthService
    {
        Task<GeneralResponse> RegisterAsync(RegisterDTO registerDTO);
        Task<GeneralResponse> LoginAsync(LoginDTO loginDTO);
        Task<GeneralResponse> LogoutAsync(string userId);
        Task<GeneralResponse> RefreshTokenAsync(TokenRequest tokenRequest);
    }
}
