using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PermissionController : ControllerBase
    {
        private readonly PermissionBusiness _PermissionBusiness;
        private readonly ILogger<PermissionController> _logger;

        public PermissionController(PermissionBusiness permissionBusiness, ILogger<PermissionController> logger)
        {
            _PermissionBusiness = permissionBusiness;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PermissionDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllPermission()
        {
            try
            {
                var Permissions = await _PermissionBusiness.GetAllPermissionAsync();
                return Ok(Permissions);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener permisos");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IEnumerable<PermissionDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetPermissionById(int id)
        {
            try
            {
                var Permission = await _PermissionBusiness.GetPermissionByIdAsync(id);
                return Ok(Permission);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el permiso con ID: {PermissionId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Permiso no encontrado con ID: {PermissionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener permiso con ID: {PermissionId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(PermissionDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateRol([FromBody] PermissionDto PermissionDto)
        {
            try
            {
                var createdPermission = await _PermissionBusiness.CreatePermissionAsyc(PermissionDto);
                return CreatedAtAction(nameof(GetPermissionById), new { id = createdPermission.Id }, createdPermission);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear permiso");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear permiso");
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PermissionDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateForm(int id, [FromBody] PermissionDto permissionDto)
        {
            if (id != permissionDto.Id)
            {
                return BadRequest(new { message = "El ID de la URL no coincide con el ID del permission" });
            }

            try
            {
                var updatePermission = await _PermissionBusiness.UpdatePermissionAsync(permissionDto);
                return Ok(updatePermission);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar permission");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Form no encontrado con ID {permissionId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar permissionDto con ID {permissionId}", id);
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
                var success = await _PermissionBusiness.DeletePermissionLogicalAsync(id);
                if (!success)
                    return NotFound(new { message = "permission no encontrado o ya inactivo" });

                return Ok(new { message = "permission eliminado lógicamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el permission con ID {id}", id);
                return StatusCode(500, new { message = "Error interno al eliminar el permission" });
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
                var success = await _PermissionBusiness.DeletePermissionPersistentAsync(id);
                if (!success)
                    return NotFound(new { message = "permission no encontrado o ya eliminado" });

                return Ok(new { message = "permission eliminado permanentemente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el permission con ID {id}", id);
                return StatusCode(500, new { message = "Error interno al eliminar el permission" });
            }
        }
    }
}
