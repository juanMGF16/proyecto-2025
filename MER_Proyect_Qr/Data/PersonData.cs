using Entity.Context;
using Entity.DTOs.Mostrar;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class PersonData
    { 
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PersonData> _logger;

        public PersonData(ApplicationDbContext context, ILogger<PersonData> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Listar todas las personas SQL
        public async Task<IEnumerable<MostrarPersonDto>> GetAllAsync()
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
        public async Task<Person?> GetByIdAsync(int id)
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

                return await _context.QueryFirstOrDefaultAsync<Person>(Query, new { Id = id });
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



        //Metodo para obtener todo de PersonaLinQ

        public async Task<IEnumerable<Person>> GetAllLinQAsync()
        {
            try
            {
                return await _context.Set<Person>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error al obtener Person");
                throw;
            }
        }



        //Metodo para obtener por Persona id LinQ

        public async Task<Person?> GetByIdLinQAsync(int id)
        {
            try
            {
                return await _context.Set<Person>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la persona con ID {PersonId}", id);
                throw; // Relanza la excepcion  para q sea manejada por las capas superiores
            }
        }

        //Metodo para crear una persona LinQ
        public async Task<Person> CreateLinQAsync(Person person)
        {
            try
            {
                await _context.Set<Person>().AddAsync(person);
                await _context.SaveChangesAsync();
                return person;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la persona: {ex.Message}");
                throw;
            }
        }

        //Metodo para actualizar a una persona LinQ
        public async Task<bool> UpdateLinQAsync(Person person)
        {
            if (person == null)
            {
                _logger.LogWarning("Intento de actualizar una persona que no existe.");
                return false;
            }
            try
            {

                _context.Set<Person>().Update(person);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la persona: {ex.Message}");
                throw;
            }
        }


        //Metodo para eliminar una persona logico LinQ
        public async Task<bool> DeleteLogicLinQAsync(int id)
        {
            try
            {
                var person = await GetByIdLinQAsync(id);
                if (person == null)
                {
                    return false;
                }

                person.Active = false;
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el person con ID {id}", id);
                throw;
            }
        }

        //Metodo para Borrar persistente LinQ
        public async Task<bool> DeletePersistenceLinQAsync(int id)
        {
            try
            {
                var person = await GetByIdLinQAsync(id);
                if (person == null)
                {
                    _logger.LogWarning("Intento de eliminar una persona que no existe (ID: {PersonId})", id);
                    return false;
                }

                _context.Set<Person>().Remove(person);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar la persona: {ex.Message}");
                throw;
            }
        }
    }
}
