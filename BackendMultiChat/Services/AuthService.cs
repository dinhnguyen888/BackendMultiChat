using AutoMapper;
using BackendMultiChat.Data;
using BackendMultiChat.Dtos;
using BackendMultiChat.Helpers;
using BackendMultiChat.Interfaces;
using BackendMultiChat.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendMultiChat.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IAccountService _accountService;
        public AuthService(AppDbContext context, IMapper mapper, ITokenService tokenService, IAccountService accountService)
        {
            _context = context;
            _mapper = mapper;
            _tokenService = tokenService;
            _accountService = accountService;
        }

        public async Task<TokenResponseDto> LoginAsync(LoginDto login)
        {
            var account = await _context.Accounts
                .Where(a => a.Email == login.Email)
                .FirstOrDefaultAsync(); 

            if (account == null || !PasswordHelper.VerifyPassword(login.Password, account.Password))
            {
               throw new UnauthorizedAccessException("Invalid email or password");
            }

            var accountDto = _mapper.Map<AccountGetDto>(account);
            var accessToken = _tokenService.GenerateAccessToken(accountDto);
            var refreshToken = await _tokenService.GenerateRefreshToken(account.AccountId);
            return new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

        }

        public async Task<string> RefreshTokenAsync(Guid accountId, string token)
        {
            var isRefreshTokenValid = await _tokenService.ValidateRefreshToken(accountId,token);
            var account = await _accountService.GetAccountByIdAsync(accountId);

            if (isRefreshTokenValid == false || account == null)
            {
                throw new UnauthorizedAccessException("Invalid refresh token or accountId");
            }
            
            var accessToken = _tokenService.GenerateAccessToken(account);

            return accessToken;
           
        }

    }
}

























//namespace BackendMultiChat.Services
//{
//    public interface IAuthService
//    {
//        Task<Contact?> LoginAsync(string phoneNumber, string password);
//        Task<(bool success, string message, Contact? contact)> RegisterAsync(string fullName, string phoneNumber, string password);
//    }

//    public class AuthService : IAuthService
//    {
//        private readonly AppDbContext _context;

//        public AuthService(AppDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<Contact?> LoginAsync(string phoneNumber, string password)
//        {
//            if (string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(password))
//            {
//                return null;
//            }

//            return await _context.Contacts
//                .SingleOrDefaultAsync(c => c.PhoneNumber == phoneNumber && c.Password == password);
//        }

//        public async Task<(bool success, string message, Contact? contact)> RegisterAsync(string fullName, string phoneNumber, string password)
//        {
//            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(phoneNumber) || string.IsNullOrEmpty(password))
//            {
//                return (false, "Invalid registration data.", null);
//            }

//            // Check if user exists
//            var existingUser = await _context.Contacts.SingleOrDefaultAsync(c => c.PhoneNumber == phoneNumber);
//            if (existingUser != null)
//            {
//                return (false, "Phone number already in use.", null);
//            }

//            // Create new user
//            var newUser = new Contact
//            {
//                FullName = fullName,
//                PhoneNumber = phoneNumber,
//                Password = password
//            };

//            _context.Contacts.Add(newUser);
//            await _context.SaveChangesAsync();

//            // Create conversations with existing contacts
//            var existingContacts = await _context.Contacts
//                .Where(c => c.ContactId != newUser.ContactId)
//                .ToListAsync();

//            foreach (var contact in existingContacts)
//            {
//                var newConversation = new Conversation
//                {
//                    ConversationName = null
//                };
//                _context.Conversations.Add(newConversation);
//                await _context.SaveChangesAsync();

//                // Add both users to the conversation
//                var members = new[]
//                {
//                    new GroupMember
//                    {
//                        ContactId = contact.ContactId,
//                        ConversationId = newConversation.ConversationId,
//                        JoinedDateTime = DateTime.UtcNow
//                    },
//                    new GroupMember
//                    {
//                        ContactId = newUser.ContactId,
//                        ConversationId = newConversation.ConversationId,
//                        JoinedDateTime = DateTime.UtcNow
//                    }
//                };

//                _context.GroupMembers.AddRange(members);
//                await _context.SaveChangesAsync();
//            }

//            return (true, "Registration successful.", newUser);
//        }
//    }
//}