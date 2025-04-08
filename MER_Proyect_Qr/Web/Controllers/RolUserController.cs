
using Business;
using Entity.DTOs;
using Entity.DTOs.Update;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RolUserController : ControllerBase
    {
        private readonly RolUserBusiness _RolUserBusiness;
        private readonly ILogger<RolUserController> _Logger;

        public RolUserController(RolUserBusiness rolUserBusiness, ILogger<RolUserController> logger)
        {
            _RolUserBusiness = rolUserBusiness;
            _Logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RolUserDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllRolUsers()
        {
            try
            {
                var RolUsers = await _RolUserBusiness.GetAllRolUserAsync();
                return Ok(RolUsers);
            }
            catch (ExternalServiceException ex)
            {
                _Logger.LogError(ex, "Error al obtner RolForm");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RolUserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRolUserByIdAsync(int id)
        {
            try
            {
                var RolUser = await _RolUserBusiness.GetRolUserByIdAsync(id);
                return Ok(RolUser);
            }
            catch (ValidationException ex)
            {
                _Logger.LogWarning(ex, "Validación fallida por el RolUser con ID: {RolUserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _Logger.LogInformation(ex, "RolUser no encontrado con ID: {RolUser}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _Logger.LogError(ex, "Error al obtner RolUser con ID: {RolUserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpPost]
        [ProducesResponseType(typeof(RolUserDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]

        public async Task<IActionResult> CreateRolUser([FromBody] RolUserDto RolUserDto)
        {
            try
            {
                var createdRolUser = await _RolUserBusiness.CreateRolUserAsync(RolUserDto);
                return CreatedAtAction(nameof(GetRolUserByIdAsync), new { id = createdRolUser.Id }, createdRolUser);
            }
            catch (ValidationException ex)
            {
                _Logger.LogWarning(ex, "Validació fallida al crear RolUser");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _Logger.LogError(ex, "Error al crear RolUser");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RolUserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateForm(int id, [FromBody] UpdateRolUserDto rolUserDto)
        {
            if (id != rolUserDto.Id)
            {
                return BadRequest(new { message = "El ID de la URL no coincide con el ID del formulario" });
            }

            try
            {
                var updatedForm = await _RolUserBusiness.UpdateRolUser(rolUserDto);
                return Ok(updatedForm);
            }
            catch (ValidationException ex)
            {
                _Logger.LogWarning(ex, "Validación fallida al actualizar Form");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _Logger.LogInformation(ex, "Form no encontrado con ID {FormId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _Logger.LogError(ex, "Error al actualizar Form con ID {FormId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPatch("delete/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteLogic(int id)
        {
            try
            {
                var success = await _RolUserBusiness.DeleteFormLogicalAsync(id);
                if (!success)
                    return NotFound(new { message = "RolUser no encontrado o ya inactivo" });

                return Ok(new { message = "RolUser eliminado lógicamente" });
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, "Error al eliminar lógicamente el RolUser con ID {id}", id);
                return StatusCode(500, new { message = "Error interno al eliminar el RolUser" });
            }
        }

        [HttpDelete("persistent/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeletePersistent(int id)
        {
            try
            {
                var success = await _RolUserBusiness.DeleteFormPersistentAsync(id);
                if (!success)
                    return NotFound(new { message = "RolUser no encontrado o ya eliminado" });

                return Ok(new { message = "RolUser eliminado permanentemente" });
            }
            catch (Exception ex)
            {
                _Logger.LogError(ex, "Error al eliminar permanentemente el RolUser con ID {id}", id);
                return StatusCode(500, new { message = "Error interno al eliminar el RolUser" });
            }
        }
    }
}
