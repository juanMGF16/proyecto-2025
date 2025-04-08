using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web.Controllers
{
    /// <summary>
    /// Controlador para la gestión de permisos en el sistema
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class RolController : ControllerBase
    {
        private readonly RolBusiness _RolBusiness;
        private readonly ILogger<RolController> _logger;

        ///<summary>
        ///constructor del controller
        /// </summary>
        /// <param name="RolBusinnes">capa de negocio de permisos</param>
        /// <param name="logger">logger para el registro de eventos</param>
        public RolController(RolBusiness RolBusiness, ILogger<RolController> logger)
        {
            _RolBusiness = RolBusiness;
            _logger = logger;
        }

        ///<summary>
        ///obtiene los permisos del sistema
        /// </summary>
        ///<returns>Lista de permisos</returns>
        /// <response code="200">Retorna la lista de permisos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RolDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllRols()
        {
            try
            {
                var Rols = await _RolBusiness.GetAllRolesAsync();
                return Ok(Rols);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener permisos");
                return StatusCode(500, new { message = ex.Message });
            } 
        }

        /// <summary>
        /// Obtiene un permiso específico por su ID
        /// </summary>
        /// <param name="id">ID del permiso</param>
        /// <returns>Permiso solicitado</returns>
        /// <response code="200">Retorna el permiso solicitado</response>
        /// <response code="400">ID proporcionado no válido</response>
        /// <response code="404">Permiso no encontrado</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RolDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetRolById(int id)
        {
            try
            {
                var Rol = await _RolBusiness.GetRolByIdAsync(id);
                return Ok(Rol);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "validacion fallida para el permiso con id: {rolId}", id);
                return BadRequest(new { message = ex.Message });
            } 
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Permiso no encontrado con ID: {RolId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
            _logger.LogError(ex, "Error al obtener permiso con ID: {RolId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo permiso en el sistema
        /// </summary>
        /// <param name="RolDto">Datos del permiso a crear</param>
        /// <returns>Permiso creado</returns>
        /// <response code="201">Retorna el permiso creado</response>
        /// <response code="400">Datos del permiso no válidos</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [ProducesResponseType(typeof(RolDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> CreateRol([FromBody] RolDto RolDto)
        {
            try
            {
                var createRol = await _RolBusiness.CreateRolAsync(RolDto);
                return CreatedAtAction(nameof(GetRolById), new { id = createRol.Id }, createRol);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validacion fallida al crear permiso");
                return BadRequest(new { messsage = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "error al crear permiso");
                return StatusCode(500, new { messsage = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RolDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateRol(int id, [FromBody] RolDto rolDto)
        {
            if (id != rolDto.Id)
            {
                return BadRequest(new { message = "El ID de la URL no coincide con el ID del formulario" });
            }

            try
            {
                var updatedRol = await _RolBusiness.UpdateRolAsync(rolDto);
                return Ok(updatedRol);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar rol");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "rol no encontrado con ID {rol}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar rol con ID {rol}", id);
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
                var success = await _RolBusiness.DeleteRolLogicalAsync(id);
                if (!success)
                    return NotFound(new { message = "Formulario no encontrado o ya inactivo" });

                return Ok(new { message = "Formulario eliminado lógicamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el rol con ID {id}", id);
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
                var success = await _RolBusiness.DeleteRolPersistentAsync(id);
                if (!success)
                    return NotFound(new { message = "Formulario no encontrado o ya eliminado" });

                return Ok(new { message = "Formulario eliminado permanentemente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el rol con ID {id}", id);
                return StatusCode(500, new { message = "Error interno al eliminar el formulario" });
            }
        }
    }
}
