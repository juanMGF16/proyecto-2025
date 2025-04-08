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
    class UserDataMySql : IUserData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserDataMySql> _logger;

        public UserDataMySql(ApplicationDbContext context, ILogger<UserDataMySql> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<MostrarUserDto>> GetAllAsyncSQL()
        {
            try
            {

                string Query = @"SELECT 
                        U.`Id`,
                        U.`UserName`, 
                        U.`Password`,
                        U.`CreationDate`,
                        U.`Active`,
                        P.`Id` AS `PersonId`,
                        CONCAT(P.`FirstName`, ' ', P.`LastName`) AS `PersonName`
                    FROM `User` AS U
                    INNER JOIN `Person` P 
                    ON P.`Id` = U.`PersonId`
                    WHERE U.`Active` = 1;";


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
                        U.`Id`,
                        U.`UserName`, 
                        U.`Password`,
                        U.`CreationDate`,
                        U.`Active`,
                        P.`Id` AS `PersonId`,
                        CONCAT(P.`FirstName`, ' ', P.`LastName`) AS `PersonName`
                    FROM `User` AS U
                    INNER JOIN `Person` P 
                    ON P.`Id` = U.`PersonId`
                    WHERE U.`Id` = @Id;";



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
                    INSERT INTO `User` (`UserName`, `Password`, `CreationDate`, `Active`, `PersonId`)
                    VALUES(@UserName, @Password, @CreationDate, @Active, @PersonId);
                    SELECT LAST_INSERT_ID();";


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
                string Query = @"UPDATE `User` 
                        SET 
                            `UserName` = @UserName, 
                            `Password` = @Password, 
                            `Active` = @Active, 
                            `PersonId` = @PersonId
                        WHERE `Id` = @Id;";



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
                string Query = @"UPDATE `User`
                            SET `Active` = 0
                            WHERE `Id` = @id;";



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
                string Query = @"DELETE FROM `User`
                        WHERE `Id` = @Id;";


                int rowAffected = await _context.ExecuteAsync(Query, new { id });
                return rowAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar user: {ex.Message}");
                return false;
            }
        }
    }
}
