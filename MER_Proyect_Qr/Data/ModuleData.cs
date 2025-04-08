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
    public class ModuleData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ModuleData> _logger;
        public ModuleData(ApplicationDbContext context, ILogger<ModuleData> logger)
        {
            _context = context;
            _logger = logger;
        }

        //Metodo para traer todo SQL
        public async Task<IEnumerable<Module>> GetAllAsync()
        {

            try
            {
                string query = @"SELECT 
                                Id,
                                Name, 
                                Description,   
                                CreationDate, 
                                Active
                            FROM Module 
                            WHERE Active = 1;";
                return await _context.QueryAsync<Module>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Los module");
                throw; // Relanza la excepcion  para q sea manejada por las capas superiores
            }


        }


        //Metodo para traer por id SQL
        public async Task<Module?> GetByIdAsync(int id)
        {
            try
            {
                string query = @"SELECT 
                                Id, 
                                Name, 
                                Description, 
                                CreationDate, 
                                Active 
                            FROM Module 
                            WHERE Id = @Id AND Active = 1";
                return await _context.QueryFirstOrDefaultAsync<Module>(query, new { Id = id });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el module con ID {ModuleId}", id);
                throw; // Relanza la excepcion  para q sea manejada por las capas superiores
            }
        }


        //Metodo para crear SQL
        public async Task<Module> CreateAsync(Module module)
        {
            try
            {
                string query = @"
                                INSERT INTO Module (Name, Description, CreationDate, Active) 
                                OUTPUT INSERTED.Id 
                                VALUES (@Name, @Description, @CreationDate, @Active);";

                module.Id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    module.Name,
                    module.Description,
                    module.CreationDate,
                    module.Active
                });

                return module;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el módulo.");
                throw;
            }
        }

        //Metodo para actualizar SQL

        public async Task<bool> UpdateAsync(Module module)
        {
            try
            {
                string query = @"
                                UPDATE Module 
                                SET Name = @Name, 
                                    Description = @Description
                                WHERE Id = @Id;";

                int rowsAffected = await _context.ExecuteAsync(query, new
                {
                    module.Id,
                    module.Name,
                    module.Description,
                    module.Active
                });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el módulo con ID {ModuleId}", module.Id);
                throw;
            }
        }


        //Metodo para borrar logico SQL
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"UPDATE Module
                               SET Active = 0
                               WHERE Id=@Id";

                int rowsAffected = await _context.ExecuteAsync(query, new { id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar logicamente module: {ex.Message}");
                return false;
            }
        }

        //Metodo para borrar persistente SQL
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = @"
                               DELETE FROM Module
                               WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(query, new { Id = id });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar module: {ex.Message}");
                return false;
            }
        }





        //Metodo para obtener Todo LinQ
        public async Task<IEnumerable<Module>> GetAllLinQAsync()

        {
            try
            {
                return await _context.Set<Module>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error al obtener form");
                throw;
            }

        }


        //Metodo para obtener por id LinQ

        public async Task<Module?> GetByIdLinQAsync(int id)
        {
            try
            {
                return await _context.Set<Module>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la modulea con ID {ModuleId}", id);
                throw; // Relanza la excepcion  para q sea manejada por las capas superiores
            }
        }

        //Metodo para crear LinQ
        public async Task<Module> CreateLinQAsync(Module module)
        {
            try
            {
                await _context.Set<Module>().AddAsync(module);
                await _context.SaveChangesAsync();
                return module;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la modulea: {ex.Message}");
                throw;
            }
        }

        //Metodo para actualizar LinQ
        public async Task<bool> UpdateLinQAsync(Module module)
        {
            try
            {
                _context.Set<Module>().Update(module);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la modulea: {ex.Message}");
                throw;
            }
        }


        //Metodo para borrar logico LinQ
        public async Task<bool> DeleteLogicLinQAsync(int id)
        {
            try
            {
                var module = await GetByIdLinQAsync(id);
                if (module == null)
                {
                    return false;
                }

                // Marcar como eliminado lógicamente
                module.Active = false; // O module.Active = 0; si es un campo numérico
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el module con ID {id}", id);
                throw;
            }
        }

        //Metodo para Borrar persistente LinQ
        public async Task<bool> DeletePersistenceLinQAsync(int id)
        {
            try
            {
                var module = await GetByIdLinQAsync(id);
                if (module == null)
                {
                    return false;
                }
                _context.Set<Module>().Remove(module);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar la modulea: {ex.Message}");
                throw;
            }
        }
    }
}
