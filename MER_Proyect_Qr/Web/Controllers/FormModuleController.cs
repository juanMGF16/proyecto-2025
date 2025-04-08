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
    public class FormModuleController : ControllerBase
    {
        private readonly FormModuleBusiness _FormModuleBusiness;
        private readonly ILogger<FormModuleController> _logger;

        public FormModuleController(FormModuleBusiness formModuleBusiness, ILogger<FormModuleController> logger)
        {
            _FormModuleBusiness = formModuleBusiness;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FormModuleDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllFormModule()
        {
            try
            {
                var FormModule = await _FormModuleBusiness.GetAllFormModuleAsync();
                return Ok(FormModule);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener permisos");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FormModuleDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetFormModuleById(int id)
        {
            try
            {
                var FormModule = await _FormModuleBusiness.GetFormModuleByIdAsync(id);
                return Ok(FormModule);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el FormModule con ID: {FormModule}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "FormModule no encontrado con ID: {FormMouduleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener FormModule con ID: {FormModuleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpPost]
        [ProducesResponseType(typeof(FormModuleDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]

        public async Task<IActionResult> CreateFormModule([FromBody] FormModuleDto FormModuleDto)
        {
            try
            {
                var createdFormModule = await _FormModuleBusiness.CreateFormModuleAsync(FormModuleDto);
                return CreatedAtAction(nameof(GetFormModuleById), new { id = createdFormModule.Id }, createdFormModule);// Ojo

            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al crear FormModule");
                return BadRequest(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al crear FormModule");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut]
        [ProducesResponseType(typeof(FormModuleDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateFormModuleDto(int id, [FromBody] UpdateFormModuleDto FormModuleDto)
        {
            if (id != FormModuleDto.Id)
            {
                return BadRequest(new { message = "El ID no coincide con el ID del RolFormPermissionBusiness" });
            }

            try
            {
                var updatedRolFormPermission = await _FormModuleBusiness.UpdateFormModuleAsync(FormModuleDto);
                return Ok(updatedRolFormPermission);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar FormModule");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "RolFormPermission no encontrado con ID {FormModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar RolFormPermission con ID {FormModuleId}", id);
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
                var success = await _FormModuleBusiness.DeleteFormModuleLogicalAsync(id);
                if (!success)
                    return NotFound(new { message = "FormModule no encontrado o ya inactivo" });

                return Ok(new { message = "Formulario eliminado lógicamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el FormModule con ID {id}", id);
                return StatusCode(500, new { message = "Error interno al eliminar el FormModule" });
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
                var success = await _FormModuleBusiness.DeleteFormModulePersistentAsync(id);
                if (!success)
                    return NotFound(new { message = "FormModule no encontrado o ya eliminado" });

                return Ok(new { message = "FormModule eliminado permanentemente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el FormModule con ID {id}", id);
                return StatusCode(500, new { message = "Error interno al eliminar el FormModule" });
            }
        }
    }
}
