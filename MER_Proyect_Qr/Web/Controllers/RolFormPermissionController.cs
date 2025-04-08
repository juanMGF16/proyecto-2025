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
    public class RolFormPermissionController : ControllerBase
    {
        private readonly RolFormPermissionBusiness _RolFormPermissionBusiness;
        private readonly ILogger<RolFormPermissionController> _logger;

        public RolFormPermissionController(RolFormPermissionBusiness rolFormPermissionBusiness, ILogger<RolFormPermissionController> logger)
        {
            _RolFormPermissionBusiness = rolFormPermissionBusiness;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RolFormPermissionDto>), 200)]
        [ProducesResponseType(500)]

        public async Task<IActionResult> GetAllRolFormPermission()
        {
            try
            {
                var RolFormPermission = await _RolFormPermissionBusiness.GetAllRolFormPermissionAsync();
                return Ok(RolFormPermission);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener permisos");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RolFormPermissionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRolFormPermissioById(int id)
        {
            try
            {
                var RolFormPermission = await _RolFormPermissionBusiness.GetRolFormPermissionByIdAsync(id);
                return Ok(RolFormPermission);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el RolFormPermission con ID: {RolFormPermissionId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "RolFormPermission no encontrado con ID: {RolFormPermission}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener RolFormPermission con ID: {RolFormPermissioId}", id);
                return StatusCode(500, new { message = ex.Message });
            }

        }


        [HttpPost]
        [ProducesResponseType(typeof(RolFormPermissionDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]

        public async Task<IActionResult> CreateRolFormPermission(RolFormPermissionDto RolFormPermissionDto)
        {
            try
            {
                var createdRolFormPermission = await _RolFormPermissionBusiness.CreateRolFormPermissionAsync(RolFormPermissionDto);
                return CreatedAtAction(nameof(GetRolFormPermissioById), new { id = createdRolFormPermission }, createdRolFormPermission);

            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear RolFormPermission");
                return BadRequest(new { messge = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear RolFormPermissio");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(RolFormPermissionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateRolFormPermission(int id, [FromBody] UpdateRolFormPermissionDto RolFormPermissionDto)
        {
            if (id != RolFormPermissionDto.Id)
            {
                return BadRequest(new { message = "El ID no coincide con el ID del RolFormPermissionBusiness" });
            }

            try
            {
                var updatedRolFormPermission = await _RolFormPermissionBusiness.UpdateRolFormPermission(RolFormPermissionDto);
                return Ok(updatedRolFormPermission);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar RolFormPermission");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "RolFormPermission no encontrado con ID {RolFormPermissionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar RolFormPermission con ID {RolFormPermissionId}", id);
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
                var success = await _RolFormPermissionBusiness.DeleteRolFormPermissionLogicalAsync(id);
                if (!success)
                    return NotFound(new { message = "RolFormPermission no encontrado o ya inactivo" });

                return Ok(new { message = "RolFormPermission eliminado lógicamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el RolFormPermission con ID {id}", id);
                return StatusCode(500, new { message = "Error interno al eliminar el RolFormPermission" });
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
                var success = await _RolFormPermissionBusiness.DeleteRolFormPermissionPersistentAsync(id);
                if (!success)
                    return NotFound(new { message = "RolFormPermission no encontrado o ya eliminado" });

                return Ok(new { message = "RolFormPermission eliminado permanentemente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el RolFormPermission con ID {id}", id);
                return StatusCode(500, new { message = "Error interno al eliminar el RolFormPermission" });
            }
        }
    }
}
