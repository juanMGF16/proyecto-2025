using Business;
using Entity.DTOs;
using Microsoft.AspNetCore.Mvc;
using Utilities.Exceptions;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly UserBusiness _UserBusiness;
        public readonly ILogger<UserController> _logger;

        public UserController(UserBusiness UserBusiness, ILogger<UserController> logger)
        {
            _UserBusiness = UserBusiness;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var Users = await _UserBusiness.GetAllUsersAsync();
                return Ok(Users);
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex ,"error al obtener servicios");
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var User = await _UserBusiness.GetUserByIdAsync(id);
                return Ok(User);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida para el permiso con ID: {UserId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "Permiso no encontrado con ID: {UserId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch(ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al obtener permiso con ID: {UserId}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateUser([FromBody] UserDto UserDto)
        {
            try
            {
                var createUser = await _UserBusiness.CreateUserAsync(UserDto);
                return CreatedAtAction(nameof(GetUserById), new { id = createUser.Id }, createUser);
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
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateForm(int id, [FromBody] UserDto userDto)
        {
            if (id != userDto.Id)
            {
                return BadRequest(new { message = "El ID de la URL no coincide con el ID del formulario" });
            }

            try
            {
                var updatedUser = await _UserBusiness.UpdateUserAsync(userDto);
                return Ok(updatedUser);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validación fallida al actualizar user");
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogInformation(ex, "user no encontrado con ID {user}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (ExternalServiceException ex)
            {
                _logger.LogError(ex, "Error al actualizar user con ID {user}", id);
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
                var success = await _UserBusiness.DeleteUserLogicalAsync(id);
                if (!success)
                    return NotFound(new { message = "Formulario no encontrado o ya inactivo" });

                return Ok(new { message = "Formulario eliminado lógicamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el user con ID {id}", id);
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
                var success = await _UserBusiness.DeleteUserPersistentAsync(id);
                if (!success)
                    return NotFound(new { message = "Formulario no encontrado o ya eliminado" });

                return Ok(new { message = "Formulario eliminado permanentemente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el user con ID {id}", id);
                return StatusCode(500, new { message = "Error interno al eliminar el formulario" });
            }
        }
    }
}
