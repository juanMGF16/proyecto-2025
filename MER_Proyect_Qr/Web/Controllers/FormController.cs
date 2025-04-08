using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class FormController : ControllerBase
    {
        private readonly FormBusiness _FormBusiness;
        private readonly ILogger<FormController> _logger;

        public FormController(FormBusiness FormBisness, ILogger<FormController> logger)
        {
            _FormBusiness = FormBisness;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<FormDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllForm()
        {
            try
            {
                var Forms = await _FormBusiness.GetAllFormAsync();
                return Ok(Forms);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener permisos");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FormDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetFormById(int id)
        {
            try
            {
                var form = await _FormBusiness.GetFormByIdAsync(id);
                return Ok(form);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el permiso con ID: {FormId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Permiso no encontrado con ID: {FormId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener permiso con ID: {FormId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(FormDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateForm([FromBody] FormDto FormDto)
        {
            try
            {
                var createForm = await _FormBusiness.CreateFormAsync(FormDto);
                return CreatedAtAction(nameof(GetFormById), new { id = createForm.Id }, createForm);
            }
            catch(ValidationException ex)
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
        [ProducesResponseType(typeof(FormDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateForm(int id, [FromBody] FormDto formDto)
        {
            if (id != formDto.Id)
            {
                return BadRequest(new { message = "El ID de la URL no coincide con el ID del formulario" });
            }

            try
            {
                var updatedForm = await _FormBusiness.UpdateFormAsync(formDto);
                return Ok(updatedForm);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar Form");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Form no encontrado con ID {FormId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar Form con ID {FormId}", id);
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
                var success = await _FormBusiness.DeleteFormLogicalAsync(id);
                if (!success)
                    return NotFound(new { message = "Formulario no encontrado o ya inactivo" });

                return Ok(new { message = "Formulario eliminado lógicamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el Form con ID {id}", id);
                return StatusCode(500, new { message = "Error interno al eliminar el formulario" });
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
                var success = await _FormBusiness.DeleteFormPersistentAsync(id);
                if (!success)
                    return NotFound(new { message = "Formulario no encontrado o ya eliminado" });

                return Ok(new { message = "Formulario eliminado permanentemente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el Form con ID {id}", id);
                return StatusCode(500, new { message = "Error interno al eliminar el formulario" });
            }
        }
    }
}
