using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data
{
    public class PermissionData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PermissionData> _logger;

        public PermissionData(ApplicationDbContext context, ILogger<PermissionData> logger)
        {
            _context = context;
            _logger = logger;
        }
        // ================================================
        // Métodos SQL
        // ================================================

        //Metodo para traer todo lo de Permiso SQL
        public async Task<IEnumerable<Permission>> GetAllAsync()
        {

            try
            {
                string query = @"SELECT 
                            Id, 
                            Name, 
                            Description, 
                            Active 
                        FROM Permission 
                        WHERE Active = 1";
                return await _context.QueryAsync<Permission>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Los permissiones");
                throw; // Relanza la excepcion  para q sea manejada por las capas superiores
            }
        }


        //Metodo para traer un permiso por id SQL
        public async Task<Permission?> GetByIdAsync(int id)
        {
            try
            {
                string query = @"SELECT 
                                    Id, 
                                    Name, 
                                    Description, 
                                    Active 
                                FROM Permission 
                                WHERE Id = @Id AND Active = 1";
                return await _context.QueryFirstOrDefaultAsync<Permission>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el permission con ID {PermissionId}", id);
                throw; // Relanza la excepcion  para q sea manejada por las capas superiores
            }
        }


        //Metodo para crear un Permiso SQL
        public async Task<Permission> CreateAsync(Permission permission)
        {
            try
            {
                string query = @"
                    INSERT INTO Permission (Name, Description, Active) 
                    OUTPUT INSERTED.Id 
                    VALUES (@Name, @Description, @Active);"
                ;

                permission.Id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    permission.Name,
                    permission.Description,
                    Active = true // Incia true
                });

                return permission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el permiso.");
                throw;
            }
        }

        //Metodo para actualizar a un permiso SQL

        public async Task<bool> UpdateAsync(Permission permission)
        {
            try
            {
                string query = @"
                                UPDATE Permission 
                                SET 
                                    Name = @Name, 
                                    Description = @Description,
                                    Active = @Active 
                                WHERE Id = @Id;";

                int rowsAffected = await _context.ExecuteAsync(query, new
                {
                    permission.Id,
                    permission.Name,
                    permission.Description,
                    permission.Active
                });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el permiso con ID {PermissionId}", permission.Id);
                throw;
            }
        }


        //Metodo para eliminar un permiso logico SQL
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"UPDATE Permission
                               SET Active = 0
                               WHERE Id=@Id";

                int rowsAffected = await _context.ExecuteAsync(query, new { id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar logicamente permission: {ex.Message}");
                return false;
            }
        }

        //Metodo para eliminar un permiso persistente SQL
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = @"
                               DELETE FROM Permission
                               WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(query, new { Id = id });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar permission: {ex.Message}");
                return false;
            }
        }

        // ================================================
        // Métodos LINQ
        // ================================================

        //Metodo para obtener todo de permisionLinQ
        public async Task<IEnumerable<Permission>> GetAllLinQAsync()
        {
            return await _context.Set<Permission>().ToListAsync();
        }


        //Metodo para obtener por id de permiso LinQ

        public async Task<Permission?> GetByIdLinQAsync(int id)
        {
            try
            {
                return await _context.Set<Permission>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la permissiona con ID {PermissionId}", id);
                throw; // Relanza la excepcion  para q sea manejada por las capas superiores
            }
        }

        //Metodo para crear un permiso  LinQ
        public async Task<Permission> CreateLinQAsync(Permission permission)
        {
            try
            {
                await _context.Set<Permission>().AddAsync(permission);
                await _context.SaveChangesAsync();
                return permission;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la permissiona: {ex.Message}");
                throw;
            }
        }

        //Metodo para actualizar un permiso  LinQ
        public async Task<bool> UpdateLinQAsync(Permission permission)
        {
            try
            {
                _context.Set<Permission>().Update(permission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la permissiona: {ex.Message}");
                throw;
            }
        }


        //Metodo para eliminar un permiso logico LinQ
        public async Task<bool> DeleteLogicLinQAsync(int id)
        {
            try
            {
                var permission = await GetByIdLinQAsync(id);
                if (permission == null)
                {
                    return false;
                }

                permission.Active = false; // O permission.Active = 0; si es un campo numérico
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el permission con ID {id}", id);
                throw;
            }
        }

        //Metodo para eliminar un permiso persistente LinQ
        public async Task<bool> DeletePersistenceLinQAsync(int id)
        {
            try
            {
                var permission = await GetByIdLinQAsync(id);
                if (permission == null)
                {
                    return false;
                }
                _context.Set<Permission>().Remove(permission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar la permissiona: {ex.Message}");
                throw;
            }
        }
    }
}
