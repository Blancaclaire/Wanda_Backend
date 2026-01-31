using Microsoft.AspNetCore.Mvc;
using DTOs;
using Models;
using wandaAPI.Repositories;
using wandaAPI.Services;

namespace wandaAPI.Controllers
{

    [Route("api")]
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
            try
            {
                var Objective = await _service.GetByAccountAsync(accountId);
                return Ok(Objective);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

        }


   
        [HttpGet("objectives/{objectiveId}")]
        public async Task<ActionResult<Objective>> GetById(int objectiveId)
        {
            
            if (objectiveId <= 0) 
                return BadRequest("El ID del objetivo no es válido.");

            try
            {            
                var objective = await _service.GetByIdAsync(objectiveId);
                
                return Ok(objective);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("accounts/{accountId}/objectives")]
        public async Task<IActionResult> Create(int accountId, [FromBody] ObjectiveCreateDto dto)
        {
            try
            {

                return Ok(await _service.CreateAsync(accountId, dto));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }


        [HttpPut("{objectiveId}")]
        public async Task<IActionResult> UpdateUser(int objectiveId, [FromBody] ObjectiveUpdateDto updatedObjective)
        {

            if (objectiveId <= 0) return BadRequest("El ID no es válido");

            try
            {
                var existingUser = await _service.GetByIdAsync(objectiveId);
                if (existingUser == null)
                {
                    return NotFound();
                }

                await _service.UpdateAsync(objectiveId, updatedObjective);
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



        [HttpDelete("objectives/{objectiveId}")]
        public async Task<IActionResult> Delete(int objectiveId)
        {
            try
            {
                if (objectiveId <= 0)
                {
                    return BadRequest("El ID no es válido");
                }
                await _service.DeleteAsync(objectiveId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }


        }
    }
}