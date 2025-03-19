using BackendMultiChat.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BackendMultiChat.Interfaces
{
    public interface IAccountService
    {
        Task<IEnumerable<AccountGetDto>> GetAllAccountsAsync();
        Task<AccountGetDto?> GetAccountByIdAsync(Guid id);
        Task<AccountGetDto> CreateAccountAsync(AccountPostDto dto);
        Task<List<AccountViewOnlineDto>> ViewOnlineAccountAsync();
        Task<bool> UpdateAccountAsync(Guid id, AccountUpdateDto dto);
        Task<bool> DeleteAccountAsync(Guid id);
        Task<bool> ChangeAdminPermissionAsync(Guid id);
    }
}
