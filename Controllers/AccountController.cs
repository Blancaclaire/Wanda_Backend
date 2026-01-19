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

        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccountById(int id)
        {
            var account = await _accountService.GetByIdAsync(id);
            if (account == null) return NotFound("Cuenta no encontrada");
            return Ok(account);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAccount([FromBody] JointAccountCreateDto account, int ownerId)
        {

            await _accountService.AddJointAccountAsync(account, ownerId);
            return Ok("Joint Account creada exitosamente");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody]  AccountUpdateDto accountDto)
        {
            try
            {

                if (id <= 0) return BadRequest("El ID no es válido");

                var existingAccount = await _accountService.GetByIdAsync(id);
                if (existingAccount == null)
                {
                    return NotFound();
                }

                await _accountService.UpdateAsync(id, accountDto);
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


    }
}