using System.Security;
using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Utilities.Exceptions;

namespace Business
{
    public class PermissionBusiness
    {
        private readonly PermissionData _permissionData;
        private readonly ILogger<PermissionBusiness> _logger;

        public PermissionBusiness(PermissionData permissionData, ILogger<PermissionBusiness> logger)
        {
            _permissionData = permissionData;
            _logger = logger;
        }


        //Método para obtener todos los Permisos como DTOs
        public async Task<IEnumerable<PermissionDto>> GetAllPermissionAsync()
        {
            try
            {

                var permissions = await _permissionData.GetAllAsync();
                var permissionsDTO = MapToDtoList(permissions);

                return permissionsDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los permisos");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de permisos", ex);
            }
        }

        // Método para obtener un usuario por ID como DTO
        public async Task<PermissionDto> GetPermissionByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un permiso con ID inválido: {PermissionId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del permiso debe ser mayor que cero");
            }

            try
            {
                var permission = await _permissionData.GetByIdAsync(id);
                if (permission == null)
                {
                    _logger.LogInformation("No se encontró ningún permission con ID: {PermissionId}", id);
                    throw new EntityNotFoundException("permission", id);
                }

                return MapToDTO(permission);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error al obtener permiso con ID {PermissionId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el permiso con ID {id}", ex);
            }

        }

        //Método para crear un Permiso desde un DTO
        public async Task<PermissionDto> CreatePermissionAsyc(PermissionDto permissionDto)
        {
            try
            {
                ValidatePermission(permissionDto);
                var permission = MapToEntity(permissionDto);

                var permissionCreated = await _permissionData.CreateAsync(permission);

                return MapToDTO(permissionCreated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo permiso: {PermissionNombre}", permissionDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear permiso", ex);

            }
        }


        public async Task<PermissionDto> UpdatePermissionAsync(PermissionDto permissionDto)
        {
            try
            {
                ValidatePermission(permissionDto);

                var existingForm = await _permissionData.GetByIdAsync(permissionDto.Id);

                if (existingForm == null)
                    throw new EntityNotFoundException("permission", permissionDto.Id);

                existingForm = MapToEntity(permissionDto);
                var update = await _permissionData.UpdateAsync(existingForm);

                if (!update)
                    throw new ExternalServiceException("Base de datos", "No se pudo actualizar el Permission");


                return MapToDTO(existingForm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el Permission con ID {permissionDto?.Id}");
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el Permission con ID {permissionDto?.Id}", ex);
            }
        }

        public async Task<bool> DeletePermissionLogicalAsync(int id)
        {
            ValidateId(id);

            try
            {
                var existingForm = await _permissionData.GetByIdAsync(id);
                if (existingForm == null)
                    throw new EntityNotFoundException("Permission", id);

                return await _permissionData.DeleteLogicAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el Permission con ID {PermissionId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el Permission", ex);
            }
        }


        public async Task<bool> DeletePermissionPersistentAsync(int id)
        {
            ValidateId(id);

            try
            {
                var existingForm = await _permissionData.GetByIdAsync(id);
                if (existingForm == null)
                    throw new EntityNotFoundException("Permission", id);

                return await _permissionData.DeletePersistenceAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el Permission con ID {PermissionId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar permanentemente el Permission", ex);
            }
        }


        private void ValidateId(int id)
        {
            if (id <= 0)
                throw new ValidationException("id", "El ID del Form debe ser mayor que cero");
        }


        // Método para validar un
        private void ValidatePermission(PermissionDto permissionDto)
        {
            if (permissionDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto permiso no puede ser nulo");
            }
            if (string.IsNullOrWhiteSpace(permissionDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un permiso con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El nombre del permiso no puede ser nulo o vacío");

            }
        }

        // Método para mapear el Permission a PermissionDto
        private PermissionDto MapToDTO(Permission permission)
        {
            return new PermissionDto
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.Description,
                Active = permission.Active
            };
        }

        // Método para mapear de PermissionDTO a Permission
        private Permission MapToEntity(PermissionDto permissionDto)
        {
            return new Permission
            {
                Id = permissionDto.Id,
                Name = permissionDto.Name,
                Description = permissionDto.Description,
                Active = permissionDto.Active
            };
        }

        // Método para mapear una lista de Usuario a una lista de UserDto
        private IEnumerable<PermissionDto> MapToDtoList(IEnumerable<Permission> permissions)
        {

            var permissionsDto = new List<PermissionDto>();
            foreach (var permission in permissions)
            {
                permissionsDto.Add(MapToDTO(permission));
            }
            return permissionsDto;
        }

    }
}
