

using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    public class ModuleBusiness
    {

        private readonly ModuleData _moduleData;
        private readonly ILogger<ModuleBusiness> _logger;

        public ModuleBusiness(ModuleData moduleData, ILogger<ModuleBusiness> logger)
        {
            _moduleData = moduleData;
            _logger = logger;
        }

        // Método para obtener todos los Modulos como DTOs
        public async Task<IEnumerable<ModuleDto>> GetAllModuleAsync()
        {
            try
            {
                var modules = await _moduleData.GetAllAsync();
                var moduleDto = MapToDtoList(modules);
                return moduleDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los formularios");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de modulos", ex);
            }
        }

        //  Método para obtner un modulo por ID como DTO 
        public async Task<ModuleDto> GetModuleByIdAsync(int id)
        {
            if(id <= 0)
            {
                _logger.LogWarning("Se intentó obtener un usuario con ID inválido: {ModuleId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del usuario debe ser mayoro qeu cero");
            }

            try
            {
                var module = await _moduleData.GetByIdAsync(id);
                if (module == null)
                {
                    _logger.LogWarning("No se encontró un modulo con ID {ModuleId}", id);
                    throw new EntityNotFoundException("Module", id);
                }
               
                return MapToDTO(module);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener modulo con ID {ModuleId}", id);
                throw new ExternalServiceException("Base de datos", "Erro al crear module", ex);
            }
        }

        // Método para crear un modulo desde un DTO

        public async Task<ModuleDto> CreateModuleAsync(ModuleDto moduleDto)
        {
            try
            {
                ValidateModule(moduleDto);
                var module = MapToEntity(moduleDto);

                var moduleCreated = await _moduleData.CreateAsync(module);

                return MapToDTO(moduleCreated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo module: {moduleNombre}", moduleDto?.Name ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear modulo", ex);
            }
        }

        public async Task<ModuleDto> UpdateModuleAsync(ModuleDto moduleDto)
        {
            try
            {
                ValidateModule(moduleDto);

                var existingForm = await _moduleData.GetByIdAsync(moduleDto.Id);

                if (existingForm == null)
                    throw new EntityNotFoundException("Form", moduleDto.Id);

                existingForm = MapToEntity(moduleDto);
                var update = await _moduleData.UpdateAsync(existingForm);

                if (!update)
                    throw new ExternalServiceException("Base de datos", "No se pudo actualizar el Form");


                return MapToDTO(existingForm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el Form con ID {moduleDto?.Id}");
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el Form con ID {moduleDto?.Id}", ex);
            }
        }

        public async Task<bool> DeleteModuleLogicalAsync(int id)
        {
            ValidateId(id);

            try
            {
                var existingForm = await _moduleData.GetByIdAsync(id);
                if (existingForm == null)
                    throw new EntityNotFoundException("Module", id);

                return await _moduleData.DeleteLogicAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el Module con ID {Module}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el Module", ex);
            }
        }

        public async Task<bool> DeleteModulePersistentAsync(int id)
        {
            ValidateId(id);

            try
            {
                var existingForm = await _moduleData.GetByIdAsync(id);
                if (existingForm == null)
                    throw new EntityNotFoundException("Module", id);

                return await _moduleData.DeletePersistenceAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el Form con ID {FormId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar permanentemente el Form", ex);
            }
        }


        // Método para validar un modulo 
        private void ValidateModule(ModuleDto moduleDto)
        {
            if (moduleDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto module no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(moduleDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un modulo con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El nombre de modulo no puede ser nulo o vacío");
            }
        }

        private void ValidateId(int id)
        {
            if (id <= 0)
                throw new ValidationException("id", "El ID  debe ser mayor que cero");
        }

        // Método para mapear el Module a ModuleDto
        private ModuleDto MapToDTO(Module module)
        {
            return new ModuleDto
            {
                Id = module.Id,
                Name = module.Name,
                Description = module.Description,
                Active = module.Active
            };
        }

        // Método para mapear de ModuleDto a Module
        private Module MapToEntity(ModuleDto moduleDto)
        {
            return new Module
            {
                Id = moduleDto.Id,
                Name = moduleDto.Name,
                Description = moduleDto.Description,
                Active = moduleDto.Active,
                CreationDate = DateTime.Now
            };
        }

        // Método para mapear una lista de Modulos a una lista de ModuleDto
        private IEnumerable<ModuleDto> MapToDtoList(IEnumerable<Module> modules)
        {
            var moduleDto = new List<ModuleDto>();
            foreach (var module in modules)
            {
                moduleDto.Add(MapToDTO(module));
            }
            return moduleDto;
        }

    }
}
