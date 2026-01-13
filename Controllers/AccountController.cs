using Microsoft.AspNetCore.Mvc;
using Models;
using wandaAPI.Repositories;
using wandaAPI.Services;


namespace wandaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountsService _AccountsService;
        private readonly IConfiguration _configuration;

        public AccountsController(IAccountsService AccountsService, IConfiguration configuration)
        {
            _AccountsService = AccountsService;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<List<Accounts>>> GetAccounts()

        {

            var Accountss = await _AccountsService.GetAllAsync();
            return Ok(Accountss);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Accounts>> GetAccountsById(int id)
        {
            try
            {
                var Accounts = await _AccountsService.GetByIdAsync(id);
                return Ok(Accounts);
            }

            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Accounts>> CreateAccounts([FromBody] AccountsCreateDTO Accounts1)
        {
            try
            {
                await _AccountsService.AddAsync(Accounts1);
                return Ok("Accounts creado exitosamente");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccounts(int id, [FromBody] AccountsUpdateDTO updatedAccounts)
        {

            if (id <= 0) return BadRequest("El ID no es válido");

            try
            {
                var existingAccounts = await _AccountsService.GetByIdAsync(id);
                if (existingAccounts == null)
                {
                    return NotFound();
                }

                await _AccountsService.UpdateAsync(id, updatedAccounts);
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
        public async Task<IActionResult> DeleteAccounts(int id)
        {

            try
            {
                await _AccountsService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {

                return NotFound(ex.Message);
            }
        }


    }
}