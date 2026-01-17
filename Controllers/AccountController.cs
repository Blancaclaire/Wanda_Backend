using Microsoft.AspNetCore.Mvc;
using Models;
using wandaAPI.Repositories;
using wandaAPI.Services;

namespace wandaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _accountRepository;

        public AccountController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Account>>> GetAccounts()
        {
            var accounts = await _accountRepository.GetAllAsync();
            return Ok(accounts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccountById(int id)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account == null) return NotFound("Cuenta no encontrada");
            return Ok(account);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAccount([FromBody] Account account)
        {

            await _accountRepository.AddAsync(account);
            return Ok("Account creada exitosamente");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] Account updatedAccount)
        {
            try
            {
                var existingAccount = await _accountRepository.GetByIdAsync(id);
                if (existingAccount == null)
                {
                    return NotFound($"No se encontró la cuenta con ID {id}");
                }


                existingAccount.Name = updatedAccount.Name;
                existingAccount.Account_Type = updatedAccount.Account_Type;
                existingAccount.Amount = updatedAccount.Amount;
                existingAccount.Weekly_budget = updatedAccount.Weekly_budget;
                existingAccount.Monthly_budget = updatedAccount.Monthly_budget;
                existingAccount.Account_picture_url = updatedAccount.Account_picture_url;

                await _accountRepository.UpdateAsync(existingAccount);
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
                await _accountRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }


    }
}