using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Entity.DTOs;
using Entity.DTOs.Mostrar;
using Entity.Model;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging;
using Utilities.Exceptions;

namespace Business
{
    public class PersonBusiness
    {
        private readonly PersonData _personData;
        private readonly ILogger<PersonBusiness> _logger;

        public PersonBusiness(PersonData personData, ILogger<PersonBusiness> logger)
        {
            _personData = personData;
            _logger = logger;
        }


        // Método para obtener todos los usuarios como DTOs
        public async Task<IEnumerable<MostrarPersonDto>> GetAllPersonAsync()
        {
            try
            {
                var persons = await _personData.GetAllAsync();
                //var personDto = MapToDTOList(persons);
                return persons;
            } 
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error al obtener todos las personas");
                throw new ExternalServiceException("Base de datos", "Error al recuperar la lista de personas", ex);

            }
        }

        // Método para obtener un persona por ID como DTO
        public async Task<PersonDto> GetPersonByIdAsync(int id)
        {
            if (id <= 0) 
            {
                _logger.LogWarning("Se intentó obtener una persona con ID inválida: {PersonId}", id);
                throw new Utilities.Exceptions.ValidationException("id", "El ID de persona debe ser mayor que cero");
            }

            try
            {
                var person = await _personData.GetByIdAsync(id);
                if (person == null)
                {
                    _logger.LogInformation("No se encontró ningúna persoan con ID: {PersonId}", id);
                    throw new EntityNotFoundException("Person", id);
                }

                return MapToDTO(person);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la persona con ID: {PersonId}", id);
                throw new ExternalServiceException("Base de datos", $"Error al recuper el con ID {id}", ex);
            }
        }

        //Método para crear una persona desde DTO
        public async Task<PersonDto> CreateAsync(PersonDto personDto)
        {
            try
            {
                ValidatePerson(personDto);

                var person = MapToEntity(personDto);

                var personCreated = await _personData.CreateAsync(person);

                return MapToDTO(personCreated);
            }
            catch (Exception ex)
            {
            _logger.LogError(ex, "Error al crear nueva persona: {PersonNombare}", personDto?.FirstName ?? "null");
                throw new ExternalServiceException("Base de datos", "Error al crear el rol", ex);


            }
        }


        public async Task<PersonDto> UpdatePersonAsync(PersonDto personDto)
        {
            try
            {
                ValidatePerson(personDto);

                var existingPerson = await _personData.GetByIdAsync(personDto.Id);

                if (existingPerson == null)
                    throw new EntityNotFoundException("person", personDto.Id);

                existingPerson = MapToEntity(personDto);
                var update = await _personData.UpdateAsync(existingPerson);

                if (!update)
                    throw new ExternalServiceException("Base de datos", "No se pudo actualizar el person");


                return MapToDTO(existingPerson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el person con ID {personDto?.Id}");
                throw new ExternalServiceException("Base de datos", $"Error al actualizar el person con ID {personDto?.Id}", ex);
            }
        }


        public async Task<bool> DeletePersonLogicalAsync(int id)
        {
            ValidateId(id);

            try
            {
                var existingPerson = await _personData.GetByIdAsync(id);
                if (existingPerson == null)
                    throw new EntityNotFoundException("Formulario", id);

                return await _personData.DeleteLogicAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el persons con ID {personId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar el person", ex);
            }
        }


        public async Task<bool> DeletePersonPersistentAsync(int id)
        {
            ValidateId(id);

            try
            {
                var existingPerson = await _personData.GetByIdAsync(id);
                if (existingPerson == null)
                    throw new EntityNotFoundException("person", id);

                return await _personData.DeletePersistenceAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente el persons con ID {personId}", id);
                throw new ExternalServiceException("Base de datos", "Error al eliminar permanentemente el persons", ex);
            }
        }


        private void ValidateId(int id)
        {
            if (id <= 0)
                throw new ValidationException("id", "El ID del person debe ser mayor que cero");
        }

        private void ValidatePerson(PersonDto personDto)
        {
            if (personDto == null)
            {
                throw new Utilities.Exceptions.ValidationException("El objeto persona no puede ser nulo");
            }

            if (string.IsNullOrWhiteSpace(personDto.FirstName))
            {
                _logger.LogWarning("Se intentó crear/actualizar una persona con Name vacío");
                throw new Utilities.Exceptions.ValidationException("Name", "El Name de persona es obligatorio");
            }

            // Mas validaciones 
        }

        //Método para mapear de Person a PersonDTO 
        private PersonDto MapToDTO(Person person)
        {
            return new PersonDto
            {
                Id = person.Id,
                FirstName = person.FirstName,
                LastName = person.LastName,
                Phone = person.PhoneNumber,
                Email = person.Email,
                Active = person.Active,

            };
        }


        //Método para mapear de PersonDTO a Person
        private Person MapToEntity(PersonDto personDto)
        {
            return new Person
            {
                Id = personDto.Id,
                FirstName = personDto.FirstName,
                LastName = personDto.LastName,
                PhoneNumber = personDto.Phone,
                Email = personDto.Email,
                Active = personDto.Active,

            };
        }

        // Método para mapear una lista de Person a una lista de PersonDTO
        private IEnumerable<PersonDto> MapToDTOList(IEnumerable<Person> persons)
        {
            var personsDTO = new List<PersonDto>();
            foreach (var person in persons) 
            {
                personsDTO.Add(MapToDTO(person));
            }
            return personsDTO;
        }
    }
}
