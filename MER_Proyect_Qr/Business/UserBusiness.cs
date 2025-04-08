

using Data;
using Data.Interfaces;
using Entity.DTOs;
using Entity.DTOs.Mostrar;
using Entity.DTOs.Update;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    public class UserBusiness
    {
        private readonly IUserData<User, MostrarUserDto> _userData;
        private readonly ILogger<UserBusiness> _logger;

        public UserBusiness(IUserData<User, MostrarUserDto> userData, ILogger<UserBusiness> logger)
        {
            _userData = userData;
            _logger = logger;
        }

        //Método para obtener todos los usuarios como DTOs
        public async Task<IEnumerable<MostrarUserDto>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userData.GetAllAsyncSQL();
                //var usersDTO = MapToDtoList(users);

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los usuarios");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de usuarios", ex);
            }
        }

        // Método para obtener un usuario por ID como DTO
        public async Task<MostrarUserDto> GetUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un usuario con ID inválido: {UserId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del usuario debe ser mayor que cero");
            }


            try
            {
                var user = await _userData.GetByIdAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("No se encontró un usuario con ID {UserId}", id);
                    throw new EntityNotFoundException("Usuario", id);
                }
                //return MapToDTO(user);
                return user;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario con ID {UserId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el usuario con ID { id }", ex);
            }
        }

        // Método para crear un usuario desde un DTO
        public async Task<UserDto> CreateUserAsync(UserDto UserDto)
        {

            try
            {
                ValidateUser(UserDto);
                var user = MapToEntity(UserDto);

                var userCreated = await _userData.CreateAsync(user);
                return MapToDTO(userCreated);
         
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo rol: {UserNombre}", UserDto?.UserName ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear usuario", ex);
            } 
        }

        public async Task<UserDto> UpdateUserAsync(UserDto userDto)
        {
            try
            {
                ValidateUser(userDto);

                var existingUser = await _userData.GetByIdAsync(userDto.Id);
                if (existingUser == null)
                    throw new EntityNotFoundException("User", userDto.Id);

                var userEntity = MapToEntity(userDto);

                var update = await _userData.UpdateAsync(userEntity);
                if (!update)
                    throw new ExternalServiceException("Base de datos", "No se pudo actualizar el Usuario");

                return MapToDTO(userEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el Usuario con ID {userDto?.Id}");
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el Usuario con ID {userDto?.Id}", ex);
            }
        }

        // Método para eliminar un Formulario Logicamente 
        public async Task<bool> DeleteUserLogicalAsync(int id)
        {
            ValidateId(id);

            try
            {
                var existingUser = await _userData.GetByIdAsync(id);
                if (existingUser == null)
                    throw new EntityNotFoundException("Formulario", id);

                return await _userData.DeleteLogicAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el user con ID {user}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el user", ex);
            }
        }


        //  Método para eliminar un Formulario de manera persistente 
        public async Task<bool> DeleteUserPersistentAsync(int id)
        {
            ValidateId(id);

            try
            {
                var existingUser = await _userData.GetByIdAsync(id);
                if (existingUser == null)
                    throw new EntityNotFoundException("user", id);

                return await _userData.DeletePersistenceAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el user con ID {user}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar permanentemente el user", ex);
            }
        }


        // Método para validar un usuario
        private void ValidateUser(UserDto userDto)
        {
            if (userDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto usuario no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(userDto.UserName))
            {
                _logger.LogWarning("Se intentó crear/actualizar un usuario con Name vacío");
                throw new Utilities.Exceptions.ValidationException("NameUser", "El nombre de usuario no puede ser nulo o vacío");       
            }             
        }

        private void ValidateId(int id)
        {
            if (id <= 0)
                throw new ValidationException("id", "El ID del user debe ser mayor que cero");
        }

      
        // Método para mapear el User a UserDto
        private UserDto MapToDTO(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Password = user.Password, 
                Active = user.Active,
                PersonId = user.PersonId,
            };
        }


        // Método para mapear de UserDTO a User
        private User MapToEntity(UserDto userDto)
        {
            return new User
            {
                Id = userDto.Id,
                UserName = userDto.UserName,
                Password = userDto.Password,
                Active = userDto.Active,
                PersonId = userDto.PersonId,
                //Person = new Person { Id = userDto.PersonId, FirstName = userDto.PersonName }
            };
        }

        // Método para mapear una lista de Usuario a una lista de UserDto
        private IEnumerable<UserDto> MapToDtoList(IEnumerable<User> users)
        {
            
            var usersDto = new List<UserDto>();
            foreach (var user in users)
            {
                usersDto.Add(MapToDTO(user));
            }
            return usersDto;
        }
     }
}
