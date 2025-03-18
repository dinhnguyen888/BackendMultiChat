using BackendMultiChat.Dtos;

namespace BackendMultiChat.Interfaces
{
    public interface IAuthService
    {
        Task<TokenResponseDto> LoginAsync(LoginDto login);
        Task<string> RefreshTokenAsync(Guid accountId, string token);
        Task<bool> LogoutAsync(string refreshToken);
    }
}