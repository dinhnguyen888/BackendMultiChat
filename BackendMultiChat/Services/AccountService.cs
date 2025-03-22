using BackendMultiChat.Data;
using BackendMultiChat.Dtos;
using BackendMultiChat.Interfaces;
using BackendMultiChat.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using BackendMultiChat.Helpers;
using BackendMultiChat.Models;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Security.Claims;
namespace BackendMultiChat.Services
{
    public class AccountService : IAccountService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPresenceHub _presenceHub;
        private readonly ITokenService _tokenService;

        public AccountService(AppDbContext context, IMapper mapper, IPresenceHub presenceHub, ITokenService tokenService)
        {
            _context = context;
            _mapper = mapper;
            _presenceHub = presenceHub;
            _tokenService = tokenService;
        }

        public async Task<IEnumerable<AccountGetDto>> GetAllAccountsAsync()
        {
            var accounts = await _context.Accounts.ToListAsync();
            return _mapper.Map<IEnumerable<AccountGetDto>>(accounts);
        }

        public async Task<List<AccountViewOnlineDto>> ViewOnlineAccountAsync(string token)
        {
            var principal = _tokenService.GetPrincipalFromToken(token);
            var userId = principal.FindFirst("id")?.Value;
            if (userId == null) throw new UnauthorizedAccessException("Invalid token");

            // Get online userId from PresenceHub
            var onlineAccounts = await _presenceHub.ViewOnlineAsync();
            var onlineAccountIds = onlineAccounts
                .Select(x => x.userId)
                .ToList();

          

            Console.WriteLine(onlineAccountIds);

            
            var accounts = await _context.Accounts
                .Where(a =>
                    a.AccountId != Guid.Parse(userId)
                   ) // Filter online accounts
                .Select(a => new AccountViewOnlineDto
                {
                    AccountId = a.AccountId,
                    FullName = a.FullName,
                    Role = a.Role.ToString(),
                    IsOnline = onlineAccountIds.Contains(a.AccountId.ToString()) ? true : false
                })
                .ToListAsync();

            return accounts;
        }




        public async Task<AccountGetDto?> GetAccountByIdAsync(Guid id)
        {
            var account = await _context.Accounts.FindAsync(id);
            return account == null ? null : _mapper.Map<AccountGetDto>(account);
        }

        public async Task<AccountGetDto> CreateAccountAsync(AccountPostDto dto)
        {
            dto.Password = PasswordHelper.HashPassword(dto.Password);
            var account = _mapper.Map<Account>(dto);
            account.AccountId = Guid.NewGuid();
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();
            return _mapper.Map<AccountGetDto>(account);
        }

        public async Task<bool> UpdateAccountAsync(Guid id, AccountUpdateDto dto)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null) return false;

            _mapper.Map(dto, account);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAccountAsync(Guid id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null) return false;

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangeAdminPermissionAsync(Guid id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null) return false;
            account.Role = Account.Roles.Admin;

            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
