using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class PersonController : ControllerBase
    {
        private readonly PersonBusiness _PersonBusiness;
        private readonly ILogger<PersonController> _logger;

        public PersonController(PersonBusiness PersonBusiness, ILogger<PersonController> logger)
        {
            _PersonBusiness = PersonBusiness;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PersonDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllPerson()
        {
            try
            {
                var Person = await _PersonBusiness.GetAllPersonAsync();
                return Ok(Person);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener permisos");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PersonDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetPersonById(int id)
        {
            try
            {
                var person = await _PersonBusiness.GetPersonByIdAsync(id);
                return Ok(person);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el permiso con ID: {PersonId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Permiso no encontrado con ID: {PersonId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener permiso con ID: {PersonId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(PersonDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreatePerson([FromBody] PersonDto PersonDto)
        {
            try
            {
                var createPerson = await _PersonBusiness.CreateAsync(PersonDto);
                return CreatedAtAction(nameof(GetPersonById), new { id = createPerson.Id }, createPerson);
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
        [ProducesResponseType(typeof(PersonDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePerson(int id, [FromBody] PersonDto personDto)
        {
            if (id != personDto.Id)
            {
                return BadRequest(new { message = "El ID de la URL no coincide con el ID del formulario" });
            }

            try
            {
                var updatedPerson= await _PersonBusiness.UpdatePersonAsync(personDto);
                return Ok(updatedPerson);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar person");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "person no encontrado con ID {personid}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar person con ID {person}", id);
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
                var success = await _PersonBusiness.DeletePersonLogicalAsync(id);
                if (!success)
                    return NotFound(new { message = "Formulario no encontrado o ya inactivo" });

                return Ok(new { message = "Formulario eliminado lógicamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el person con ID {id}", id);
                return StatusCode(500, new { message = "Error interno al eliminar el person" });
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
                var success = await _PersonBusiness.DeletePersonPersistentAsync(id);
                if (!success)
                    return NotFound(new { message = "Formulario no encontrado o ya eliminado" });

                return Ok(new { message = "Formulario eliminado permanentemente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el person con ID {id}", id);
                return StatusCode(500, new { message = "Error interno al eliminar  person" });
            }
        }
    }
}
