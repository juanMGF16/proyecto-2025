
using System;
using System.Collections.Generic;
using Entity.Context;
using Entity.DTOs.Mostrar;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Data
{
    public class UserData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserData> _logger;

        // Constructor que recibe el contexto de la base de datos.
        public UserData(ApplicationDbContext context, ILogger<UserData> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Obtiene todos los usuarios almacenados en la base de datos con SQL.
        public async Task<IEnumerable<MostrarUserDto>> GetAllAsyncSQL()
        {
            try
            {

                string Query = @"SELECT 
                                U.Id,
                                U.UserName, 
								U.Password,
								U.CreationDate,
								U.Active,
                                P.Id AS PersonId,
                                CONCAT(P.FirstName, ' ', P.LastName) AS PersonName
                                FROM [User] AS U
                                INNER JOIN Person P 
                                ON P.Id = U.PersonId";
                return await _context.QueryAsync<MostrarUserDto>(Query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Los useres {User}");
                throw; // Relanza la excepcion  para q sea manejada por las capas superiores
            }
        }

        // Obtiene un usuario específico por su Id.
        public async Task<MostrarUserDto?> GetByIdAsync(int id)
        {
            try
            {
                string Query = @"SELECT 
                                    U.Id,
                                    U.UserName, 
								    U.Password,
								    U.CreationDate,
								    U.Active,
                                    P.Id AS PersonId,
                                    CONCAT(P.FirstName, ' ', P.LastName) AS PersonName
                                FROM [User] AS U
                                INNER JOIN Person P 
                                ON P.Id = U.PersonId 
                                WHERE U.Id = @Id";

                return await _context.QueryFirstOrDefaultAsync<MostrarUserDto>(Query, new { @Id = id });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario con el ID {UserId}", id);
                throw;
            }
        }


        // Método para crear un usuario SQL 
        public async Task<User> CreateAsync(User user)
        {
            try
            {
                string query = @"
                    INSERT INTO [User] (UserName, [Password], CreationDate, Active, PersonId)
                    OUTPUT INSERTED.Id
                    VALUES(@UserName, @Password, @CreationDate, @Active, @PersonId);";

                var parameters = new
                {
                    user.UserName,
                    user.Password,  // Encripta la contraseña
                    CreationDate = DateTime.UtcNow,
                    Active = true,
                    user.PersonId,
                };

                // Usa int? para evitar errores si la inserción no devuelve un ID
                user.Id = await _context.QueryFirstOrDefaultAsync<int?>(query, parameters) ?? 0;

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el user.");
                throw;
            }
        }

        // Método para actualizar usuario SQL

        public async Task<bool> UpdateAsync(User user)
        {
            try
            {
                string Query = @"UPDATE [User] 
                                SET 
                                    UserName = @UserName, 
                                    Password = @Password, 
                                    Active = @Active, 
                                    PersonId = @PersonId
                                WHERE Id = @Id;";

                int rowsAffected = await _context.ExecuteAsync(Query, new
                {
                    user.Id,
                    user.UserName,
                    user.Password,
                    user.Active,        
                    user.PersonId
                });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar el user: {ex.Message}");
                return false;
            }
        }

        // Método para eliminar usuario Logico SQL

        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string Query = @"UPDATE [User]
                                SET 
                                Active = 0
                                WHERE Id = @id";

                int rowAffected = await _context.ExecuteAsync(Query, new { id });
                return rowAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar logicamente user: {ex.Message}");
                return false;
            }
        }

        // Método para eliminar un usuario de manera Persistente SQL 
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string Query = @"DELETE FROM [User]
                                WHERE Id = @Id";
                int rowAffected = await _context.ExecuteAsync(Query, new { id });
                return rowAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar user: {ex.Message}");
                return false;
            }
        }


        // Obtiene todos los usuarios almacenados en la base de datos con LINQ.
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            try
            {
                return await _context.Set<User>()
                .Include(user => user.Person)
                .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los usuarios.");
                throw; // Relanza la excepción para que el error no se oculte.
            }
        }


        
        // Obtiene un usuario específico por su Id LINQ.
        public async Task<User?> GetByIdLinQAsync(int id)
        {
            try
            {
                return await _context.Set<User>().FindAsync(id);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario con el ID {UserId}", id);
                throw;
            }
        }
        
        // Crea un usuario LINQ
        public async Task<User> CreateAsyncLinq(User user)
        {
            try
            {
                await _context.Set<User>().AddAsync(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario");
                throw;
            }
        }

        // Actualiza un usuario
        public async Task<bool> UpdateAsyncLinq(User user)
        {
            try
            {
                _context.Set<User>().Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario");
                return false;
            }
        }

        // Método para eliminar un usuario de forma logica LINQ
        public async Task<bool> DeleteLoqicLinqAsinc(int id)
        {
            try
            {
                var user = await GetByIdLinQAsync(id);
                if (user == null)
                    return false;

                user.Active = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el user con ID {id}", id);
                throw;
            }
        }


        //  Método para eleminar un usuario de forma Persistente LINQ
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var user = await _context.Set<User>().FindAsync(id);

                if (user == null)
                    return false;
                
                _context.Set<User>().Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar usuario con el ID {UserId}", id);
                return false;
            }
        }
    }
}
