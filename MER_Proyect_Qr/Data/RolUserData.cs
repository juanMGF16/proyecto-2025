using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Context;
using Entity.DTOs;
using Entity.DTOs.Mostrar;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Data
{
    public class RolUserData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RolUserData> _logger;

        public RolUserData(ApplicationDbContext context, ILogger<RolUserData> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Listar todos los RolUser sql
        public async Task<IEnumerable<MostrarRolUserDto>> GetAllAsyncSQL()
        {
            try
            {
                string query = @"
                                SELECT 
                                    RU.Id, 
                                    RU.RolId AS RolId, 
                                    RU.UserId AS UserId, 
                                    R.Name AS RolName, 
                                    U.UserName, 
                                    RU.Active
                                FROM RolUser RU
                                INNER JOIN Rol R ON RU.RolId = R.Id
                                INNER JOIN [User] U ON RU.UserId = U.Id 
                                WHERE RU.Active = 1;";

                return await _context.QueryAsync<MostrarRolUserDto>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los roles de usuario desde SQL.");
                throw;
            }
        }

        // Obtener RolUser por Id
        public async Task<MostrarRolUserDto?> GetByIdAsync(int id)
        {
            try
            {
                string query = @"
                        SELECT 
                            RU.Id, 
                            RU.Active,
                            R.Id AS RolId,
                            R.Name AS RolName,
                            U.Id AS UserId,
                            U.UserName
                        FROM RolUser RU
                        INNER JOIN Rol R ON RU.RolId = R.Id
                        INNER JOIN [User] U ON RU.UserId = U.Id
                        WHERE RU.Id = @Id";


                return await _context.QueryFirstOrDefaultAsync<MostrarRolUserDto>(query, new { Id = id });
                //return await _context.Set<RolUser>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener RolUser con el ID {RolUserId}", id);
                throw;
            }
        }

        // Método para crear un RolUser con SQL
        public async Task<RolUser> CreateAsync(RolUser rolUser)
        {
            try
            {

                string Query = @"INSERT INTO RolUser" +
                                " (RolId, UserId, Active)" +
                                " OUTPUT INSERTED.Id" +
                                " VALUES" +
                                " (@RolId, @UserId, @Active)";

                var Parameters = new
                {
                    @RolId = rolUser.RolId,
                    @UserId = rolUser.UserId,
                    @Active = rolUser.Active,
                };

                rolUser.Id = await _context.ExecuteScalarAsync<int>(Query, Parameters);
                return rolUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"no se pudo agregar persona {rolUser}");
                throw;
            }
        }

        // Método para actualizar RolUser SQL
        public async Task<bool> UpdateAsync(RolUser rolUser)
        {

            try
            {
                string Query = @"
                            UPDATE RolUser
                            SET
                                RolId = @RolId,
                                UserId = @UserId
                            WHERE Id = @Id";

                var Parameters = new
                {
                    @Id = rolUser.Id,
                    @RolId = rolUser.RolId,
                    @UserId = rolUser.UserId,

                };
                int rowsAffected = await _context.ExecuteAsync(Query, Parameters);
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"No se pudo actualizar {rolUser}");
                throw;
            }
        }


        // Método para Eliminar un RolUser Logico SQL
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string Query = @"UPDATE RolUser
                                SET
                                    Active = 0
                                WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(Query, new { Id = id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al borra logicamente con ID {RolUserId}",id);
                throw;
            }
        }

        // Método para eleminar un RolUser con persistencia SQL
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string Query = @"DELETE FROM RolUser
                                WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(Query, new { Id = id });
                return rowsAffected > 0; // almenos una fila
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente RolUser con ID {RolUserId}", id);
                throw;
            }
        }

        // ================================================
        // Métodos LINQ
        // ================================================

        // Método para obtener todo de RolUser LINQ
        public async Task<IEnumerable<RolUser>> GetByIdLinQAsync()
        {
            try
            {

                return await _context.Set<RolUser>()
                    .Include(rol => rol.Rol)
                    .Include(rol => rol.User)
                    .Where(fm => !fm.Active)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al traer el RolUser ");
                throw;
            }
        }

        // Obtiene un RolUser específico por su Id LINQ.
        public async Task<RolUser?> GetByIdLinQAsync(int id)
        {
            try
            {
                return await _context.Set<RolUser>()
                            .Include(rol => rol.Rol)
                            .Include(rol => rol.User)
                            .FirstAsync(rol  => rol.Id == id);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario con el ID {UserId}", id);
                throw;
            }
        }

        // Crear RolUser LINQ
        public async Task<RolUser> CreateLinQAsync(RolUser rolUser)
        {
            try
            {
                await _context.Set<RolUser>().AddAsync(rolUser);
                await _context.SaveChangesAsync();
                return rolUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear RolUser");
                throw;
            }
        }
        // Actualizar RolUser
        public async Task<bool> UpdateLinQAsync(RolUser rolUser)
        {
            try
            {
                _context.Set<RolUser>().Update(rolUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar RolUser");
                return false;
            }
        }

        // Eliminar RolUser Logico LINQ
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

        // Eliminar RolUser persistente LINQ
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var rolUser = await _context.Set<RolUser>().FindAsync(id);

                if (rolUser == null)
                    return false;

                _context.Set<RolUser>().Remove(rolUser);
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
