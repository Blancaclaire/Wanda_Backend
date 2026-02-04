using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using wandaAPI.Repositories;
using wandaAPI.Services;

namespace wandaAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountRepository) //error de nombre, public AccountController(IAccountService accountService) 
        {
            _accountService = accountRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Account>>> GetAccounts()
        {
            var accounts = await _accountService.GetAllAsync();
            return Ok(accounts);
        }

        [HttpGet("{accountId}")]
        public async Task<ActionResult<Account>> GetAccountById(int accountId)
        {
            var account = await _accountService.GetByIdAsync(accountId);
            if (account == null) return NotFound("Cuenta no encontrada");
            return Ok(account);
        }

        [HttpPost]
        
        public async Task<ActionResult> CreateAccount([FromBody] JointAccountCreateDto account)
        {

            await _accountService.AddJointAccountAsync(account);
            return Ok("Joint Account creada exitosamente");
        }

        [HttpPut("{accountId}")]
        public async Task<IActionResult> UpdateAccount(int accountId, [FromBody]  AccountUpdateDto accountDto)
        {
            try
            {

                if (accountId <= 0) return BadRequest("El ID no es válido");

                var existingAccount = await _accountService.GetByIdAsync(accountId);
                if (existingAccount == null)
                {
                    return NotFound();
                }

                await _accountService.UpdateAsync(accountId, accountDto);
                return NoContent();


            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
             catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{accountId}")]
        public async Task<IActionResult> DeleteAccount(int accountId)
        {
            try
            {
                await _accountService.DeleteAsync(accountId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }


    }
}