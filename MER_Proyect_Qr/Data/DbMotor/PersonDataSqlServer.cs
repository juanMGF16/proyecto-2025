using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Interfaces;
using Entity.Context;
using Entity.DTOs.Mostrar;
using Entity.Model;
using Microsoft.Extensions.Logging;

namespace Data.DbMotor
{
    public class PersonDataSqlServer : IUserData<Person, MostrarPersonDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PersonDataSqlServer> _logger;

        public PersonDataSqlServer(ApplicationDbContext context, ILogger<PersonDataSqlServer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<MostrarPersonDto>> GetAllAsyncSQL()
        {

            try
            {
                string query = @"SELECT 
                                    Id,
                                    CONCAT(FirstName, ' ', LastName) AS NameCompleted, 
                                    PhoneNumber AS Phone, 
                                    Email,
                                    Active
                                FROM Person 
                                    WHERE  Active = 1";
                return await _context.QueryAsync<MostrarPersonDto>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Los Person");
                throw; // Relanza la excepcion  para q sea manejada por las capas superiores
            }

        }
        // Obtener Persona por Id SQL
        public async Task<MostrarPersonDto?> GetByIdAsync(int id)
        {
            try
            {
                string Query = @"SELECT 
                                    Id,
                                    FirstName, 
                                    LastName, 
                                    PhoneNumber AS Phone, 
                                    Email,
                                    Active
                                FROM Person 
                                    WHERE Id = @Id AND Active = 1";

                return await _context.QueryFirstOrDefaultAsync<MostrarPersonDto>(Query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Persona con el ID {PersonaId}", id);
                throw;
            }
        }


        // Crear Persona con SQL
        public async Task<Person> CreateAsync(Person person)
        {
            try
            {
                string query = @"
                                INSERT INTO Person (FirstName, LastName, Email, PhoneNumber, Active) 
                                OUTPUT INSERTED.Id 
                                VALUES (@FirstName, @LastName, @Email, @PhoneNumber, @Active);";

                person.Id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    person.FirstName,
                    person.LastName,
                    person.Email,
                    person.PhoneNumber,
                    Active = true
                });

                return person;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la persona.");
                throw;
            }
        }


        //Metodo para actualizar Personas con SQL

        public async Task<bool> UpdateAsync(Person person)
        {
            try
            {
                string query = @"
                                UPDATE Person 
                                SET FirstName = @FirstName, 
                                    LastName = @LastName, 
                                    Email = @Email, 
                                    PhoneNumber = @PhoneNumber
                                WHERE Id = @Id;";




                int rowsAffected = await _context.ExecuteAsync(query, new
                {
                    person.Id,
                    person.FirstName,
                    person.LastName,
                    person.Email,
                    person.PhoneNumber
                });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la persona con ID {PersonId}", person.Id);
                throw;
            }
        }

        //Metodo para eliminar un persona logico SQL
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"UPDATE Person
                               SET Active = 0
                               WHERE Id=@Id";

                int rowsAffected = await _context.ExecuteAsync(query, new { id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente la persona con ID {PersonId}", id);
                return false;
            }

        }


        //Metodo para eliminar una persona persistente SQL
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = @"
                                DELETE FROM Person
                                WHERE Id = @Id";

                int rowsAffected = await _context.ExecuteAsync(query, new { Id = id });


                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la persona.");
                return false;
            }
        }
    }
}
