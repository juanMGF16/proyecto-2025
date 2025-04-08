
using System.Threading.Tasks;
using Data;
using Entity.DTOs;
using Entity.Model;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    public class FormBusiness
    {

        private readonly FormData _formData;
        private readonly ILogger<FormBusiness> _logger;

        public FormBusiness(FormData formData, ILogger<FormBusiness> logger)
        {
            _formData = formData;
            _logger = logger;
        }

        // Método para obtner todos los formularios como DTOs
        public async Task<IEnumerable<FormDto>> GetAllFormAsync()
        {
            try
            {
                var forms = await _formData.GetAllAsync();
                var formsDto = MapToDtoList(forms);
                return formsDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los formularios");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de formularios", ex);
            }
        }

        // Método para obtener un formulario por ID como DTO
        public async Task<FormDto> GetFormByIdAsync(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Se intentó un usuario con ID inválido: {FormId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID del usuario debe ser mayor que cero");
            }

            try
            {
                var form = await _formData.GetByIdAsync(id);
                if (form == null)
                {
                    _logger.LogWarning("No se encontró un formulario con ID {FormId}", id);
                    throw new EntityNotFoundException("Formulario", id);

                }

                return MapToDTO(form);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario con ID {FormId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuperar el rol con ID {id}", ex);
            }
        }


        // Método para crear un formulario desde un DTO 
        public async Task<FormDto> CreateFormAsync(FormDto formDto)
        {
            try
            {
                validateForm(formDto);  
                var form = MapToEntity(formDto);
                var formCreated = await _formData.CreateAsync(form);

                return MapToDTO(formCreated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nuevo Form: {FormNombre}", formDto?.Name ?? "null");
                throw new ExternalServiceException("Base de dato", "Error al crear un formulario", ex);
            }
        }


        // Método para actualizar un Fomulario

        public async Task<FormDto> UpdateFormAsync(FormDto formDto)
        {
            try
            {
                validateForm(formDto);

                var existingForm = await _formData.GetByIdAsync(formDto.Id);

                if (existingForm == null)
                    throw new EntityNotFoundException("Form", formDto.Id);

                existingForm = MapToEntity(formDto);
                var update = await _formData.UpdateLinQAsync(existingForm);

                if (!update)
                    throw new ExternalServiceException("Base de datos", "No se pudo actualizar el Form");


                return MapToDTO(existingForm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el Form con ID {formDto?.Id}");
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el Form con ID {formDto?.Id}", ex);
            }
        }

        // Método para eliminar un Formulario Logicamente 
        public async Task<bool> DeleteFormLogicalAsync(int id)
        {
            ValidateId(id);

            try
            {
                var existingForm = await _formData.GetByIdAsync(id);
                if (existingForm == null)
                    throw new EntityNotFoundException("Formulario", id);

                return await _formData.DeleteLogicLinQAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el Form con ID {FormId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el Form", ex);
            }
        }


        //  Método para eliminar un Formulario de manera persistente 
        public async Task<bool> DeleteFormPersistentAsync(int id)
        {
            ValidateId(id);

            try
            {
                var existingForm = await _formData.GetByIdAsync(id);
                if (existingForm == null)
                    throw new EntityNotFoundException("Form", id);

                return await _formData.DeletePersistenceLinQAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el Form con ID {FormId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar permanentemente el Form", ex);
            }
        }

        // Método para validar un usuario
        private void validateForm(FormDto formDto)
        {
            if(formDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto usuario no puede ser nulo");
            }

            if(string.IsNullOrWhiteSpace(formDto.Name))
            {
                _logger.LogWarning("Se intentó crear/actualizar un rol con Name vacío");
                throw new Utilities.Exceptions.ValidationException("NameUser", "El nombre de usuarioo no puede ser nulo o vacío");
            }
        }

        private void ValidateId(int id)
        {
            if (id <= 0)
                throw new ValidationException("id", "El ID del Form debe ser mayor que cero");
        }

        // Método para mapear el Form a FormDto
        private FormDto MapToDTO(Form form)
        {
            return new FormDto
            {
                Id = form.Id,
                Name = form.Name,
                Description = form.Description,
                Active = form.Active
            };
        }

        // Método para mapear una lista de Form a una lista de FormDto
        private Form MapToEntity(FormDto formDto)
        {
            return new Form
            {
                Id = formDto.Id,
                Name = formDto.Name,
                Description = formDto.Description,
                Active = formDto.Active,
                CreationDate = DateTime.Now
            };
        }

        // Método para mapear una lista de Form 
        private IEnumerable<FormDto> MapToDtoList(IEnumerable<Form> forms)
        {
            var formsDto = new List<FormDto>();
            foreach (var form in forms)
            {
                formsDto.Add(MapToDTO(form));
            }
            return formsDto;
        }

    }

}
    
