using AutoMapper;
using BackendMultiChat.Data;
using BackendMultiChat.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BackendMultiChat.Services
{
    public class TodoService
    {
        private readonly AppDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        public TodoService(
            AppDbContext context,
            ITokenService tokenService,
            IAccountService accountService,
            IMapper mapper)
        {
            _context = context;
            _tokenService = tokenService;
            _accountService = accountService;
            _mapper = mapper;

        }
    

    public async Task<object> GetAllProjectAsync()
        {
            var projects = await _context.Projects.ToListAsync();
            return projects;
        }
    }
}
