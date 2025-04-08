using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class ModuleController :ControllerBase
    {
        private readonly ModuleBusiness _ModuleBusiness;
        private readonly ILogger<ModuleController> _logger;

        public ModuleController(ModuleBusiness moduleBusiness, ILogger<ModuleController> logger)
        {
            _ModuleBusiness = moduleBusiness;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ModuleDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllModule()
        {
            try
            {
                var Modules = await _ModuleBusiness.GetAllModuleAsync();
                return Ok(Modules);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener permisos");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IEnumerable<ModuleDto>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetModuleById(int id)
        {
            try
            {
                var Module = await _ModuleBusiness.GetModuleByIdAsync(id);
                return Ok(Module);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el permiso con ID: {ModuleId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Permiso no encontrado con ID: {ModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener permiso con ID: {ModuleId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ModuleDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateModule([FromBody] ModuleDto ModuleDto)
        {
            try
            {
                var createdModule= await _ModuleBusiness.CreateModuleAsync(ModuleDto);
                return CreatedAtAction(nameof(GetModuleById), new { id = createdModule.Id }, createdModule);
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
        [ProducesResponseType(typeof(ModuleDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateForm(int id, [FromBody] ModuleDto moduleDto)
        {
            if (id != moduleDto.Id)
            {
                return BadRequest(new { message = "El ID de la URL no coincide con el ID del formulario" });
            }

            try
            {
                var updatedModule= await _ModuleBusiness.UpdateModuleAsync(moduleDto);
                return Ok(updatedModule);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar Module");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Form no encontrado con ID {ModuleId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar Form con ID {ModuleId}", id);
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
                var success = await _ModuleBusiness.DeleteModuleLogicalAsync(id);
                if (!success)
                    return NotFound(new { message = "Module no encontrado o ya inactivo" });

                return Ok(new { message = "Module eliminado lógicamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el Module con ID {id}", id);
                return StatusCode(500, new { message = "Error interno al eliminar el Module" });
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
                var success = await _ModuleBusiness.DeleteModulePersistentAsync(id);
                if (!success)
                    return NotFound(new { message = "Module no encontrado o ya eliminado" });

                return Ok(new { message = "Module eliminado permanentemente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el Module con ID {id}", id);
                return StatusCode(500, new { message = "Error interno al eliminar el Module" });
            }
        }
    }
}
