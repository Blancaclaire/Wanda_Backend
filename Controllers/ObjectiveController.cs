using Microsoft.AspNetCore.Mvc;
using DTOs;
using Models;
using wandaAPI.Repositories;
using wandaAPI.Services;

namespace wandaAPI.Controllers{

    [Route("api/[controller]")]
    [ApiController]
    public class ObjectivesController : ControllerBase
    {
        private readonly IObjectiveService _service;

        public ObjectivesController(IObjectiveService service)
        {
            _service = service;
        }

        [HttpGet("accounts/{accountId}/objectives")]
        public async Task<IActionResult> GetByAccount(int accountId)
        {
            return Ok(await _service.GetByAccountAsync(accountId));
        }

        [HttpPost("accounts/{accountId}/objectives")]
        public async Task<IActionResult> Create(int accountId, [FromBody] ObjectiveCreateDto dto)
        {
            return Ok(await _service.CreateAsync(accountId, dto));
        }

        [HttpDelete("objectives/{objectiveId}")]
        public async Task<IActionResult> Delete(int objectiveId)
        {
            await _service.DeleteAsync(objectiveId);
            return NoContent();
        }
    }
}