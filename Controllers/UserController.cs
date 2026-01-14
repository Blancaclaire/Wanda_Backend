using Microsoft.AspNetCore.Mvc;
using Models;
using wandaAPI.Repositories;
using wandaAPI.Services;


namespace wandaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public UserController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetUsers()

        {

            var Users = await _userService.GetAllAsync();
            return Ok(Users);

        }
        [HttpGet("{id}")]
        public async Task<ActionResult<Account>> GetAccountById(int id)
        {

            var account = await _AccountsService.GetByIdAsync(id);
            if (account == null) return NotFound();
            return Ok(account);

        }

        [HttpPost]
        public async Task<ActionResult> CreateAccount([FromBody] AccountsCreateDTO accountDto)
        {
            try
            {
                var result = await _AccountsService.AddAsync(accountDto);

                return CreatedAtAction(nameof(GetAccountById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDTO updatedUser)
        {

            if (id <= 0) return BadRequest("El ID no es válido");

            try
            {
                var existingUser = await _userService.GetByIdAsync(id);
                if (existingUser == null)
                {
                    return NotFound();
                }

                await _userService.UpdateAsync(id, updatedUser);
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
        public async Task<IActionResult> DeleteUser(int id)
        {

            try
            {
                await _userService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {

                return NotFound(ex.Message);
            }
        }


    }
}