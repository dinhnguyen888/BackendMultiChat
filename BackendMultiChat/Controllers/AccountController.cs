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

        [HttpGet]
        public async Task<IActionResult> GetAllAccounts()
        {
            try
            {
                var accounts = await _accountService.GetAllAccountsAsync();
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Failed to get accounts: {ex.Message}" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccountById(Guid id)
        {
            try
            {
                var account = await _accountService.GetAccountByIdAsync(id);
                return account == null
                    ? NotFound(new { message = $"Account with ID {id} not found" })
                    : Ok(account);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Failed to get account: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] AccountPostDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdAccount = await _accountService.CreateAccountAsync(dto);
                return CreatedAtAction(nameof(GetAccountById), new { id = createdAccount.AccountId }, createdAccount);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Failed to create account: {ex.Message}" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(Guid id, [FromBody] AccountUpdateDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var success = await _accountService.UpdateAccountAsync(id, dto);
                return success
                    ? NoContent()
                    : NotFound(new { message = $"Account with ID {id} not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Failed to update account: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(Guid id)
        {
            try
            {
                var success = await _accountService.DeleteAccountAsync(id);
                return success
                    ? NoContent()
                    : NotFound(new { message = $"Account with ID {id} not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Failed to delete account: {ex.Message}" });
            }
        }

        [HttpPost("change-admin-permission/{id}")]
        public async Task<IActionResult> ChangeAdminPermission(Guid id)
        {
            try
            {
                var success = await _accountService.ChangeAdminPermissionAsync(id);
                return success
                    ? NoContent()
                    : NotFound(new { message = $"Account with ID {id} not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Failed to change admin permission: {ex.Message}" });
            }
        }

        [HttpGet("online")]
        public async Task<IActionResult> ViewOnlineAccounts()
        {
            try
            {
                var onlineAccounts = await _accountService.ViewOnlineAccountAsync();
                if (onlineAccounts == null || !onlineAccounts.Any())
                {
                    return NotFound(new { message = "No online accounts found" });
                }

                return Ok(onlineAccounts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Failed to get online accounts: {ex.Message}" });
            }
        }
    }
}
