using Microsoft.AspNetCore.Mvc;
using wandaAPI.Services;
using Models;

namespace wandaAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class TransactionSplitController : ControllerBase
    {
        private readonly ITransactionSplitService _transactionSplitService;

        public TransactionSplitController(ITransactionSplitService transactionSplitService)
        {
            _transactionSplitService = transactionSplitService;
        }

       [HttpGet("users/{userId}/transactionSplits")]
        public async Task<ActionResult<List<TransactionSplit>>> GetUserSplits(
            int userId, 
            [FromQuery] string? status) 
        {
            if (userId <= 0) return BadRequest("El ID de usuario es obligatorio.");

            try
            {
    
                var debts = await _transactionSplitService.GetUserSplitsAsync(userId, status);
                return Ok(debts);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //El metodo se usa para aceptar una deuda
        //AL aceptarse la deuda se generan dos nuevas transacciones: Deuda y Reembolso
        //Se actualiza el estado de la deuda a "settled"

        [HttpPost("transactionSplits/{id}/")]
        public async Task<IActionResult> AcceptDebt(int id)
        {
            if (id <= 0) return BadRequest("El ID no es válido.");

            try
            {

                await _transactionSplitService.AcceptDebtAsync(id);

                return Ok("Deuda aceptada y liquidada correctamente.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}