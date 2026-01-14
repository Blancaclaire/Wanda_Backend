using Microsoft.AspNetCore.Mvc;
using Models;
using wandaAPI.Services;

namespace wandaAPI.Controllers
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
        public async Task<ActionResult<List<Account>>> GetAccounts()
        {
            var accounts = await _accountService.GetAllAsync();
            return Ok(accounts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccountById(int id)
        {
            var account = await _accountService.GetByIdAsync(id);
            if (account == null) return NotFound("Cuenta no encontrada");
            return Ok(account);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAccount([FromBody] CreateAccountDto dto)
        {
            var account = new Account
            {
                Name = dto.Name,
                AccountType = dto.AccountType,
                Balance = dto.Balance,
                WeeklyBudget = dto.WeeklyBudget,
                MonthlyBudget = dto.MonthlyBudget,
                AccountPictureUrl = dto.AccountPictureUrl,
                Password = dto.Password
            };

            await _accountService.AddAsync(account);
            return CreatedAtAction(nameof(GetAccountById), new { id = account.AccountId }, account);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] UpdateAccountDto dto)
        {
            try
            {
                var account = new Account
                {
                    AccountId = id,
                    Name = dto.Name,
                    AccountType = dto.AccountType,
                    WeeklyBudget = dto.WeeklyBudget,
                    MonthlyBudget = dto.MonthlyBudget,
                    AccountPictureUrl = dto.AccountPictureUrl
                };

                await _accountService.UpdateAsync(account);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            try
            {
                await _accountService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("seed")]
        public async Task<IActionResult> InitializeData()
        {
            await _accountService.InicializarDatosAsync();
            return Ok("Datos inicializados correctamente");
        }
    }
}