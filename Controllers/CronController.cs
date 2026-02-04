using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using wandaAPI.Services;

namespace wandaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CronController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IConfiguration _configuration;

        public CronController(ITransactionService transactionService, IConfiguration configuration)
        {
            _transactionService = transactionService;
            _configuration = configuration;
        }

        [HttpPost("execute")]
        [AllowAnonymous]
        public async Task<IActionResult> ExecuteCron([FromHeader(Name = "X-Cron-Secret")] string secret)
        {
            // 1. Validar seguridad: Comparamos la clave recibida con la guardada en configuración
            var expectedSecret = _configuration["CronSettings:SecretKey"];

            if (string.IsNullOrEmpty(expectedSecret) || secret != expectedSecret)
            {
                return Unauthorized(new { message = "Acceso denegado. Clave incorrecta." });
            }

            try
            {
                Console.WriteLine($"[{DateTime.Now}] Ejecutando CRON de recurrencia...");

                // 2. Llamamos a tu lógica existente
                await _transactionService.ProcessRecurringTransactionsAsync();

                return Ok(new { message = "Recurrencia ejecutada con éxito.", date = DateTime.Now });
            }
            catch (Exception ex)
            {
                // Logueamos el error real en la consola del servidor
                Console.Error.WriteLine($"Error en CRON: {ex.Message}");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}