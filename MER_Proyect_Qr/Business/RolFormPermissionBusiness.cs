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
using Microsoft.SqlServer.Server;
using Utilities.Exceptions;

namespace Business
{
    public class RolFormPermissionBusiness
    {
        private readonly RolFormPermissionData _rolFormPermissionData;
        private readonly ILogger<RolFormPermissionBusiness> _logger;

        public RolFormPermissionBusiness(RolFormPermissionData rolFormPermissionData, ILogger<RolFormPermissionBusiness> logger)
        {
            _rolFormPermissionData = rolFormPermissionData;
            _logger = logger;
        }

        // Método para obtener todos los RolFormPermission como DTOs
        public async Task<IEnumerable<MostrarRolFormPermissionDto>> GetAllRolFormPermissionAsync()
        {
            try
            {
                var rolFormPermission = await _rolFormPermissionData.GetAllAsyncSQL();
                //var rolFormPermissionDTO = MapToDTOList(rolFormPermission);
                return rolFormPermission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los rolFormPermission");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de rolFormPermission", ex);
            }

        }

        // Método para obtener un RolFormPermission por ID como DTO
        public async Task<MostrarRolFormPermissionDto> GetRolFormPermissionByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intento obtener un rolFormPermission con un ID inválido: {RolFormPermissionId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del rolFormPermission debe ser mayor a 0");
            }

            try
            {
                var rolFormPermission = await _rolFormPermissionData.GetByIdAsync(id);
                if (rolFormPermission == null)
                {
                    _logger.LogInformation("No se encontro ninungo rolFormPermission con el id {RolFormPermissionId}", id);
                    throw new EntityNotFoundException("RolFormPermission", id);
                }
                //return MapToDTO(rolFormPermission);
                return rolFormPermission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rolFormPermission con ID {RolFormPermissionId}", id);
                throw new ExternalServiceException("Base de datos", "Error al recuperar el rolFormPermission", ex);
            }
        }

        //Metodo para crear un RolFormPermission desde un DTO
        public async Task<RolFormPermissionDto> CreateRolFormPermissionAsync(RolFormPermissionDto RolFormPermissionDto)
        {
            try
            {
                ValidateRolFormPermission(RolFormPermissionDto);
                var rolFormPermission = MapToEntity(RolFormPermissionDto);

                var rolFormPermissionCreado = await _rolFormPermissionData.CreateAsync(rolFormPermission);
                return MapToDTO(rolFormPermissionCreado);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error al crear el rolFormPermission: {RolFormPermissionNombre}", RolFormPermissionDto?.RolName ?? "null");
                _logger.LogError(ex, "Error al crear el rolFormPermission: {RolFormPermissionId}", RolFormPermissionDto?.Id);
                throw new ExternalServiceException("Base de datos", "Error al crear el rolFormPermission", ex);
            }

        }



        // Método para Actualizar
        public async Task<RolFormPermissionDto> UpdateRolFormPermission(UpdateRolFormPermissionDto updateDto)
        {
            try
            {
                // Validar el DTO de actualización
                ValidateUpdateRolFormPermission(updateDto);

                var existingDto = await _rolFormPermissionData.GetByIdAsync(updateDto.Id);

                if (existingDto == null)
                    throw new EntityNotFoundException("RolFormPermission", updateDto.Id);

                // Crear un RolFormPermission a partir del DTO de actualización
                var entityToUpdate = MapUpdateDtoToEntity(updateDto);

                var update = await _rolFormPermissionData.UpdateAsync(entityToUpdate);

                if (!update)
                    throw new ExternalServiceException("Base de datos", "No se pudo actualizar el RolFormPermission");

                var updateRolFormPermission = await _rolFormPermissionData.GetByIdAsync(updateDto.Id);
                if (updateRolFormPermission == null)
                {
                    throw new EntityNotFoundException("RolFormPermission", updateDto.Id);
                }
                return MapUpdateDtoToRolFormPermission(updateRolFormPermission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el RolFormPermission con ID {updateDto?.Id}");
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el RolFormPermission con ID {updateDto?.Id}", ex);
            }
        }

        // Método de validación específico para UpdateRolFormPermissionDto
        private void ValidateUpdateRolFormPermission(UpdateRolFormPermissionDto updateDto)
        {
            if (updateDto == null)
            {
                throw new ValidationException("UpdateRolFormPermission", "El DTO de actualización no puede ser nulo");
            }
        }

        // Método para eliminar un Formulario Logicamente 
        public async Task<bool> DeleteRolFormPermissionLogicalAsync(int id)
        {
            ValidateId(id);

            try
            {
                var existingForm = await _rolFormPermissionData.GetByIdAsync(id);
                if (existingForm == null)
                    throw new EntityNotFoundException("RolFormPermission", id);

                return await _rolFormPermissionData.DeleteLogicAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el RolFormPermission con ID {RolFormPermissionId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el RolFormPermission", ex);
            }
        }


        //  Método para eliminar un Formulario de manera persistente 
        public async Task<bool> DeleteRolFormPermissionPersistentAsync(int id)
        {
            ValidateId(id);

            try
            {
                var existingForm = await _rolFormPermissionData.GetByIdAsync(id);
                if (existingForm == null)
                    throw new EntityNotFoundException("RolFormPermission", id);

                return await _rolFormPermissionData.DeletePersistenceAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el RolFormPermission con ID {RolFormPermissionId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar permanentemente el RolFormPermission", ex);
            }
        }


        /// <summary>
        /// Metodo para validar si el Id Existe
        /// </summary>
        /// <param name="RolFormPermissionDto"></param>
        /// <exception cref="Utilities.Exceptions.ValidationException"></exception>
        /// 
        private void ValidateId(int id)
        {
            if (id <= 0)
                throw new ValidationException("id", "El ID del Form debe ser mayor que cero");
        }

        //Metodo para validar el DTO

        public void ValidateRolFormPermission(RolFormPermissionDto RolFormPermissionDto)
        {
            if (RolFormPermissionDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("RolFormPermission", "El rolFormPermission no puede ser nulo");
            }
            //if (string.IsNullOrWhiteSpace(RolFormPermissionDto.ReferenceEqual) && string.IsNullOrWhiteSpace(RolFormPermissionDto.RolName))
            //{
            //    _logger.LogWarning("Se intentó crear/actualizar un rolFormPermission con Name vacío");
            //    throw new Utilities.Exceptions.ValidationException("Name", "El Name del rolFormPermission es obligatorio");
            //}

        }

        private RolFormPermission MapUpdateDtoToEntity(UpdateRolFormPermissionDto dto)
        {
            return new RolFormPermission
            {
                Id = dto.Id,
                Active = true,
                RolId = dto.RolId,
                FormId = dto.FormId,
                PermissionId = dto.PermissionId
            };
        }

        private RolFormPermissionDto MapUpdateDtoToRolFormPermission(MostrarRolFormPermissionDto updateDto)
        {
            return new RolFormPermissionDto
            {
                Id = updateDto.Id,
                Active = updateDto.Active,
                RolId = updateDto.RolId,
                FormId = updateDto.FormId,
                PermissionId = updateDto.PermissionId,
            };
        }

        // Método para mapear de RolFormPermission a RolFormPermissionDTO
        private RolFormPermissionDto MapToDTO(RolFormPermission rolFormPermission)
        {
            return new RolFormPermissionDto
            {
                Id = rolFormPermission.Id,
                Active = rolFormPermission.Active,
                RolId = rolFormPermission.RolId,
                FormId = rolFormPermission.FormId,
                PermissionId = rolFormPermission.PermissionId,

                //RolName = rolFormPermission.Rol.Name,
                //FormName = rolFormPermission.Form.Name,
                //PermissionName = rolFormPermission.Permission.Name

            };
        }

        // Método para mapear de RolFormPermissionDTO a RolFormPermission
        private RolFormPermission MapToEntity(RolFormPermissionDto rolFormPermissionDto)
        {
            return new RolFormPermission
            {
                Id = rolFormPermissionDto.Id,
                Active = rolFormPermissionDto.Active,
                RolId = rolFormPermissionDto.RolId,
                FormId = rolFormPermissionDto.FormId,
                PermissionId = rolFormPermissionDto.PermissionId
            };
        }

        // Método para mapear una lista de RolFormPermission a una lista de RolFormPermissionDTO
        private IEnumerable<RolFormPermissionDto> MapToDTOList(IEnumerable<RolFormPermission> rolFormPermissions)
        {
            var rolFormPermissionsDTO = new List<RolFormPermissionDto>();
            foreach (var rolFormPermission in rolFormPermissions)
            {
                rolFormPermissionsDTO.Add(MapToDTO(rolFormPermission));
            }
            return rolFormPermissionsDTO;
        }


    }
}