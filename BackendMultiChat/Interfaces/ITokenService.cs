using BackendMultiChat.Dtos;
using System.Security.Claims;

namespace BackendMultiChat.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(AccountGetDto account);
        Task<string> GenerateRefreshToken(Guid accountId);
        ClaimsPrincipal GetPrincipalFromToken(string token);
        Task<bool> ValidateRefreshToken(Guid accountId, string refreshToken);
    }
}