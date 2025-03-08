using BackendMultiChat.Dtos;
using BackendMultiChat.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BackendMultiChat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // Get all accounts
        [HttpGet]
        public async Task<IActionResult> GetAllAccounts()
        {
            var accounts = await _accountService.GetAllAccountsAsync();
            return Ok(accounts);
        }

        // Get account by id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountById(Guid id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            return account == null ? NotFound() : Ok(account);
        }

        // Create new account
        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] AccountPostDto dto)
        {
            var createdAccount = await _accountService.CreateAccountAsync(dto);
            return CreatedAtAction(nameof(GetAccountById), new { id = createdAccount.AccountId }, createdAccount);
        }

        // Update account
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(Guid id, [FromBody] AccountUpdateDto dto)
        {
            var success = await _accountService.UpdateAccountAsync(id, dto);
            return success ? NoContent() : NotFound();
        }

        // Delete account
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(Guid id)
        {
            var success = await _accountService.DeleteAccountAsync(id);
            return success ? NoContent() : NotFound();
        }

        [HttpPost("change-admin-permission/{id}")]
        public async Task<IActionResult> ChangeAdminPermission(Guid id)
        {
            var success = await _accountService.ChangeAdminPermissionAsync(id);
            return success ? NoContent() : NotFound();
        }
    }
}
