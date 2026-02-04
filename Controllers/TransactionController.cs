using Microsoft.AspNetCore.Mvc;
using wandaAPI.Services;
using Models;
using DTOs;
using Microsoft.AspNetCore.Authorization;

namespace wandaAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }
        
        [HttpGet("accounts/{accountId}/transactions")]
        public async Task<ActionResult<List<Transaction>>> GetByAccountAsync(int accountId)
        {
            try
            {
                var transactions = await _transactionService.GetByAccountAsync(accountId);
                return Ok(transactions);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("accounts/{accountId}/transactions")]
        public async Task<ActionResult<Transaction>> CreateAsync(int accountId, [FromBody] TransactionCreateDTO dto)
        {
            try
            {
                var createdTransaction = await _transactionService.CreateAsync(accountId, dto);
                return Ok("Transaction creada");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("transactions/{id}")]
        public async Task<ActionResult<Transaction>>GetByIdAsync(int id)
        {
            try
            {
                var transaction = await _transactionService.GetByIdAsync(id);
                return Ok(transaction);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("transactions/{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] TransactionUpdateDTO dto)
        {
            try
            {
                await _transactionService.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("transactions/{id}")]
        public async Task<IActionResult>DeleteAsync(int id)
        {
            try
            {
                await _transactionService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}