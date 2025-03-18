using BackendMultiChat.Dtos;

namespace BackendMultiChat.Interfaces
{
    public interface IProfileService
    {
        Task<bool> ChangePasswordAsync(string token, string oldPassWord, string newPassword);
        Task<ProfileGetDto> GetProfileInformation(string token);
        Task<bool> UpdateProfileInformation(string token, AccountUpdateDto dto);
    }
}