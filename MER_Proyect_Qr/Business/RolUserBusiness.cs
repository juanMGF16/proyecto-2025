using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Entity.DTOs;
using Entity.DTOs.Mostrar;
using Entity.DTOs.Update;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    public class RolUserBusiness
    {
        private readonly RolUserData _rolUserData;
        private readonly ILogger<RolUserBusiness> _logger;

        public RolUserBusiness(RolUserData rolUserData, ILogger<RolUserBusiness> logger)
        {
            this._rolUserData = rolUserData;
            this._logger = logger;
        }



        // Método para obtener todos los RolUser como DTOs
        public async Task<IEnumerable<MostrarRolUserDto>> GetAllRolUserAsync()
        {
            try
            {
                var rolUser = await _rolUserData.GetAllAsyncSQL();
                //var rolUserDTO = MapToDTOList(rolUser);
                return rolUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los rolUser");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de rolUser", ex);
            }

        }


        // Método para obtener un RolUser por ID como DTO
        public async Task<MostrarRolUserDto> GetRolUserByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intento obtener un rol con un ID inválido: {RolUserId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del rolUser debe ser mayor a 0");
            }

            try
            {
                var rolUser = await _rolUserData.GetByIdAsync(id);
                if (rolUser == null)
                {
                    _logger.LogInformation("No se encontro ninungo rolUser con el id {RolUserId}", id);
                    throw new EntityNotFoundException("RolUser", id);
                }
                //return MapToDTO(rolUser);
                return rolUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rolUser con ID {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al recuperar el rolUser", ex);
            }
        }

        //Metodo para crear un RolUser desde un DTO
        public async Task<RolUserDto> CreateRolUserAsync(RolUserDto RolUserDto)
        {
            try
            {
                ValidateRolUser(RolUserDto);
                var rolUser = MapToEntity(RolUserDto);

                var rolUserCreado = await _rolUserData.CreateAsync(rolUser);
                return MapToDTO(rolUserCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el rolUser: {RolUserNombre}", RolUserDto?.Id );
                throw new ExternalServiceException("Base de datos", "Error al crear el rolUser", ex);
            }

        }


        public async Task<RolUserDto> UpdateRolUser(UpdateRolUserDto updateDto)
        {
            try
            {
                // Validar el DTO de actualización
                //ValidateRolUser(updateDto);

                var existingDto = await _rolUserData.GetByIdAsync(updateDto.Id);

                if (existingDto == null)
                    throw new EntityNotFoundException("RolUser", updateDto.Id);

                // Crear un RolFormPermission a partir del DTO de actualización
                var entityToUpdate = MapUpdateDtoToEntity(updateDto);

                var update = await _rolUserData.UpdateAsync(entityToUpdate);

                if (!update)
                    throw new ExternalServiceException("Base de datos", "No se pudo actualizar el RolFormPermission");

                var updateRolFormPermission = await _rolUserData.GetByIdAsync(updateDto.Id);
                if (updateRolFormPermission == null)
                {
                    throw new EntityNotFoundException("RolFormPermission", updateDto.Id);
                }
                return MapUpdateDtoToRolUser(updateRolFormPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el RolFormPermission con ID {updateDto?.Id}");
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el RolFormPermission con ID {updateDto?.Id}", ex);
            }
        }


        public async Task<bool> DeleteFormLogicalAsync(int id)
        {
            ValidateId(id);

            try
            {
                var existingForm = await _rolUserData.GetByIdAsync(id);
                if (existingForm == null)
                    throw new EntityNotFoundException("RolUser", id);

                return await _rolUserData.DeleteLogicAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el RolUser con ID {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el RolUser", ex);
            }
        }

        public async Task<bool> DeleteFormPersistentAsync(int id)
        {
            ValidateId(id);

            try
            {
                var existingForm = await _rolUserData.GetByIdAsync(id);
                if (existingForm == null)
                    throw new EntityNotFoundException("RolUser", id);

                return await _rolUserData.DeletePersistenceAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el RolUser con ID {RolUserId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar permanentemente el RolUser", ex);
            }
        }

        //Metodo para validar el DTO

        public void ValidateRolUser(RolUserDto rolUserDto)
        {
            if (rolUserDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("RolUser", "El rolUser no puede ser nulo");
            }

            if (rolUserDto.UserId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un rolUser con UserId inválido");
                throw new Utilities.Exceptions.ValidationException("UserId", "El UserId del rolUser es obligatorio y debe ser mayor a 0");
            }

            if (rolUserDto.RolId <= 0)
            {
                _logger.LogWarning("Se intentó crear/actualizar un rolUser con RolId inválido");
                throw new Utilities.Exceptions.ValidationException("RolId", "El RolId del rolUser es obligatorio y debe ser mayor a 0");
            }
        }

        private void ValidateId(int id)
        {
            if (id <= 0)
                throw new ValidationException("id", "El ID del Form debe ser mayor que cero");
        }


        private RolUser MapUpdateDtoToEntity(UpdateRolUserDto dto)
        {
            return new RolUser
            {
                Id = dto.Id,
                Active = true,
                RolId = dto.RolId,
                UserId = dto.UserId,
            };
        }

        private RolUserDto MapUpdateDtoToRolUser(MostrarRolUserDto updateDto)
        {
            return new RolUserDto
            {
                Id = updateDto.Id,
                Active = true,
                RolId = updateDto.RolId,
                UserId = updateDto.UserId,
            };
        }


        // Método para mapear de RolUser a RolUserDTO
        private RolUserDto MapToDTO(RolUser rolUser)
        {
            return new RolUserDto
            {
                Id = rolUser.Id,
                RolId = rolUser.RolId,
                UserId = rolUser.UserId,
                //RolName = rolUser.Rol.Name,
                //UserName = rolUser.User.UserName,
                Active = rolUser.Active,
            };
        }

        // Método para mapear de RolUserDTO a RolUser
        private RolUser MapToEntity(RolUserDto rolUserDto)
        {
            return new RolUser
            {
                Id = rolUserDto.Id,
                UserId = rolUserDto.UserId,
                RolId = rolUserDto.RolId,
                Active = rolUserDto.Active,
            };
        }

        // Método para mapear una lista de RolUser a una lista de RolUserDTO
        private IEnumerable<RolUserDto> MapToDTOList(IEnumerable<RolUser> rolUsers)
        {
            var rolUsersDTO = new List<RolUserDto>();
            foreach (var rolUser in rolUsers)
            {
                rolUsersDTO.Add(MapToDTO(rolUser));
            }
            return rolUsersDTO;
        }

    }
}
