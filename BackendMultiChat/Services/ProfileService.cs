using AutoMapper;
using BackendMultiChat.Data;
using BackendMultiChat.Dtos;
using BackendMultiChat.Helpers;
using BackendMultiChat.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;
using System.Security.Claims;

namespace BackendMultiChat.Services
{
    public class ProfileService : IProfileService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;
        public ProfileService(AppDbContext context, IMapper mapper, IAccountService accountService, ITokenService tokenService)
        {
            _context = context;
            _mapper = mapper;
            _accountService = accountService;
            _tokenService = tokenService;

        }

        public async Task<ProfileGetDto> GetProfileInformation(string token)
        {
            var claimPrincipal = _tokenService.GetPrincipalFromToken(token);


            var accountId = Guid.Parse(claimPrincipal.FindFirstValue("id"));
            var account = await _accountService.GetAccountByIdAsync(accountId);
            var accountMapping = _mapper.Map<ProfileGetDto>(account);
            return accountMapping;
        }

        public async Task<bool> UpdateProfileInformation(string token, AccountUpdateDto dto)
        {
            var claimPrincipal = _tokenService.GetPrincipalFromToken(token);
            var accountId = Guid.Parse(claimPrincipal.FindFirstValue("id"));
            var account = await _context.Accounts.SingleOrDefaultAsync(a => a.AccountId == accountId);
            if (account == null)
            {
                return false;
            }
            account.FullName = dto.FullName;
            account.Email = dto.Email;
            account.Password = PasswordHelper.HashPassword(dto.Password);
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePasswordAsync(string token, string oldPassWord, string newPassword)
        {
            var claimPrincipal = _tokenService.GetPrincipalFromToken(token);
            var accountId = Guid.Parse(claimPrincipal.FindFirstValue("id"));
            var account = await _context.Accounts.SingleOrDefaultAsync(a => a.AccountId == accountId);
            if (account == null)
            {
                return false;
            }
            if (!PasswordHelper.VerifyPassword(oldPassWord, account.Password))
            {
                return false;
            }
            account.Password = PasswordHelper.HashPassword(newPassword);
            _context.Accounts.Update(account);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
