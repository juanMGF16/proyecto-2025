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
    public class FormModuleBusiness
    {
        private readonly FormModuleData _formModuleData;
        private readonly ILogger<FormBusiness> _logger;

        public FormModuleBusiness(FormModuleData formModuleData, ILogger<FormBusiness> logger)
        {
            _formModuleData = formModuleData;
            _logger = logger;
        }

        // Método para obtener todos los FormModule como DTOs
        public async Task<IEnumerable<MostrarFormModuleDto>> GetAllFormModuleAsync()
        {
            try
            {
                var formModules = await _formModuleData.GetAllAsync();
                //var formModulesDTO = MapToDTOList(formModules);
                return formModules;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los formModules");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de formModules", ex);
            }

        }


        // Método para obtener un FormModule por ID como DTO
        public async Task<MostrarFormModuleDto> GetFormModuleByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intento obtener un rol con un ID inválido: {FormModuleId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del formModule debe ser mayor a 0");
            }

            try
            {
                var formModule = await _formModuleData.GetByIdAsync(id);
                if (formModule == null)
                {
                    _logger.LogInformation("No se encontro ninungo formModule con el id {FormModuleId}", id);
                    throw new EntityNotFoundException("FormModule", id);
                }
                //return MapToDTO(formModule);
                return formModule;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el formModule con ID {FormModuleId}", id);
                throw new ExternalServiceException("Base de datos", "Error al recuperar el formModule", ex);
            }
        }

        //Metodo para crear un FormModule desde un DTO
        public async Task<FormModuleDto> CreateFormModuleAsync(FormModuleDto FormModuleDto)
        {
            try
            {
                ValidateFormModule(FormModuleDto);
                var formModule = MapToEntity(FormModuleDto);

                var formModuleCreado = await _formModuleData.CreateAsync(formModule);
                return MapToDTO(formModuleCreado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el formModule: {FormModuleId}", FormModuleDto?.Id );
                throw new ExternalServiceException("Base de datos", "Error al crear el formModule", ex);
            }

        }

        public async Task<FormModuleDto> UpdateFormModuleAsync(UpdateFormModuleDto updateFormModuleDto)
        {
            try
            {
                //ValidateFormModule(updateFormModuleDto);

                var existingFormModule = await _formModuleData.GetByIdAsync(updateFormModuleDto.Id);

                if (existingFormModule == null)
                    throw new EntityNotFoundException("FormModuke", updateFormModuleDto.Id);

                // Mapeo manual como sugerimos antes
                var entityToUpdate = MapUpdateDtoToEntity(updateFormModuleDto);

                // Esto es lo que pasas al repositorio - asegúrate que es de tipo FormModule
                var update = await _formModuleData.UpdateAsync(entityToUpdate);

                if (!update)
                    throw new ExternalServiceException("Base de datos", "No se pudo actualizar el FormModule");

                var updatedFormModule = await _formModuleData.GetByIdAsync(updateFormModuleDto.Id);

                return MapUpdateDtoToFormModuleDto(updatedFormModule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el FormModule: {ex.Message}");
                throw new ExternalServiceException("Base de datos", ex.Message);
            }
        }

        public async Task<bool> DeleteFormModuleLogicalAsync(int id)
        {
            ValidateId(id);

            try
            {
                var existingForm = await _formModuleData.GetByIdAsync(id);
                if (existingForm == null)
                    throw new EntityNotFoundException("Formulario", id);

                return await _formModuleData.DeleteLogicAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el Form con ID {FormId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el Form", ex);
            }
        }

        //  Método para eliminar un Formulario de manera persistente 
        public async Task<bool> DeleteFormModulePersistentAsync(int id)
        {
            ValidateId(id);

            try
            {
                var existingForm = await _formModuleData.GetByIdAsync(id);
                if (existingForm == null)
                    throw new EntityNotFoundException("Form", id);

                return await _formModuleData.DeletePersistenceAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el FormModule con ID {FormModuleId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar permanentemente el FormModule", ex);
            }
        }

        // Método para validar un FormModuleDTO
        public void ValidateFormModule(FormModuleDto formModuleDto)
        {
            if (formModuleDto == null)
                throw new ValidationException("FormModule", "El FormModule no puede ser nulo.");

            if (formModuleDto.FormId <= 0)
                throw new ValidationException("FormId", "El FormId del FormModule es obligatorio y debe ser mayor a 0.");

            if (formModuleDto.ModuleId <= 0)
                throw new ValidationException("ModuleId", "El ModuleId del FormModule es obligatorio y debe ser mayor a 0.");
        }

        private void ValidateId(int id)
        {
            if (id <= 0)
                throw new ValidationException("id", "El ID del FormModule debe ser mayor que cero");
        }

        private FormModule MapUpdateDtoToEntity(UpdateFormModuleDto dto)
        {
            return new FormModule
            {
                Id = dto.Id,
                ModuleId = dto.ModuleId,
                FormId = dto.FormId,
                Active = true  // O utiliza dto.Active si existe esa propiedad
            };
        }


        private FormModuleDto MapUpdateDtoToFormModuleDto(MostrarFormModuleDto updateDto)
        {
            return new FormModuleDto
            {
                Id = updateDto.Id,
                FormId = updateDto.FormId,
                ModuleId = updateDto.ModuleId,
                Active = true  // O utiliza updateDto.Active si existe esa propiedad
            };
        }

        // Método para mapear de FormModule a FormModuleDTO
        private FormModuleDto MapToDTO(FormModule formModule)
        {
            return new FormModuleDto
            {
                Id = formModule.Id,
                FormId = formModule.FormId,
                ModuleId = formModule.ModuleId,
                //FormName = formModule.Form.Name,
                //ModuleName = formModule.Module.Name,
                Active = formModule.Active,
            };
        }

        // Método para mapear de FormModuleDTO a FormModule
        private FormModule MapToEntity(FormModuleDto formModuleDto)
        {
            return new FormModule
            {
                Id = formModuleDto.Id,
                ModuleId = formModuleDto.ModuleId,
                FormId = formModuleDto.FormId,
                Active = formModuleDto.Active,


            };
        }


        // Método para mapear una lista de FormModule a una lista de FormModuleDTO
        private IEnumerable<FormModuleDto> MapToDTOList(IEnumerable<FormModule> formModules)
        {
            var formModulesDTO = new List<FormModuleDto>();
            foreach (var formModule in formModules)
            {
                formModulesDTO.Add(MapToDTO(formModule));
            }
            return formModulesDTO;
        }


    }
}
