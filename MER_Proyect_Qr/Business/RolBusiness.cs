using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    public class RolBusiness
    {
        private readonly RolData _rolData;
        private readonly ILogger<RolBusiness> _logger;

        public RolBusiness(RolData rolData, ILogger<RolBusiness> logger)
        {
            _rolData = rolData;
            _logger = logger;
        }

        // Método para obtener todos los roles como DTOs
        public async Task<IEnumerable<RolDto>> GetAllRolesAsync()
        {
            try
            {
                var roles = await _rolData.GetAllAsync();
                //var rolesDTO = MapToDTOList(roles);

                return roles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los roles");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de roles", ex);
            }
        }

        // Método para obtener un rol por ID como DTO
        public async Task<RolDto> GetRolByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un rol con ID inválido: {RolId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del rol debe ser mayor que cero");
            }

            try
            {
                var rol = await _rolData.GetByIdAsync(id);
                if (rol == null)
                {
                    _logger.LogInformation("No se encontró ningún rol con ID: {RolId}", id);
                    throw new EntityNotFoundException("Rol", id);
                }

                return MapToDTO(rol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el rol con ID: {RolId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el rol con ID {id}", ex);
            }
        }

        // Método para crear un rol desde un DTO
        public async Task<RolDto> CreateRolAsync(RolDto RolDto)
        {
            try
            {
                ValidateRol(RolDto);

                var rol =  MapToEntity(RolDto);

                var rolCreado = await _rolData.CreateAsync(rol);

                return MapToDTO(rolCreado);
            
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo rol: {RolNombre}", RolDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el rol", ex);
            }
        }




        public async Task<RolDto> UpdateRolAsync(RolDto rolDto)
        {
            try
            {
                ValidateRol(rolDto);

                var existingRol= await _rolData.GetByIdAsync(rolDto.Id);

                if (existingRol == null)
                    throw new EntityNotFoundException("rol", rolDto.Id);

                existingRol = MapToEntity(rolDto);
                var update = await _rolData.UpdateAsync(existingRol);

                if (!update)
                    throw new ExternalServiceException("Base de datos", "No se pudo actualizar el rol");


                return MapToDTO(existingRol);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el rol con ID {rolDto?.Id}");
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el rol con ID {rolDto?.Id}", ex);
            }
        }

        // Método para eliminar un Formulario Logicamente 
        public async Task<bool> DeleteRolLogicalAsync(int id)
        {
            ValidateId(id);

            try
            {
                var existingRol= await _rolData.GetByIdAsync(id);
                if (existingRol == null)
                    throw new EntityNotFoundException("rol", id);

                return await _rolData.DeleteLogicAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el rol con ID {rolId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el rol", ex);
            }
        }


        //  Método para eliminar un Formulario de manera persistente 
        public async Task<bool> DeleteRolPersistentAsync(int id)
        {
            ValidateId(id);

            try
            {
                var existingRol = await _rolData.GetByIdAsync(id);
                if (existingRol == null)
                    throw new EntityNotFoundException("rol", id);

                return await _rolData.DeletePersistenceAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el rol con ID {rol}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar permanentemente el rol", ex);
            }
        }

        private void ValidateId(int id)
        {
            if (id <= 0)
                throw new ValidationException("id", "El ID del rol debe ser mayor que cero");
        }



        // Método para validar el DTO
        private void ValidateRol(RolDto RolDto)
        {
            if (RolDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto rol no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(RolDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un rol con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name del rol es obligatorio");
            }
        }


        // Método para mapear de Rol a RolDTO
        private RolDto MapToDTO(Rol rol)

        {
            return new RolDto
            {
                Id = rol.Id,
                Name = rol.Name,
                Code = rol.Code,
                Active = rol.Active,
                Description = rol.Description
            };
        }

        // Método para mapear de RolDTO a Rol
        private Rol MapToEntity(RolDto rolDTO)
        {
            return new Rol
            {
                Id = rolDTO.Id,
                Name = rolDTO.Name,
                Code = rolDTO.Code,
                Description = rolDTO.Description,
                Active = rolDTO.Active // Si existe en la entidad
            };
        }

        // Método para mapear una lista de Rol a una lista de RolDTO
        private IEnumerable<RolDto> MapToDTOList(IEnumerable<Rol> roles)
        {
            var rolesDTO = new List<RolDto>();
            foreach (var rol in roles)
            {
                rolesDTO.Add(MapToDTO(rol));
            }
            return rolesDTO;
        }
    }
}
