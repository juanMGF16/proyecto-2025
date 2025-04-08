using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.Context;
using Entity.DTOs;
using Entity.DTOs.Mostrar;
using Entity.DTOs.Update;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Data
{
    public class FormModuleData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FormModuleData> _logger;

        public FormModuleData(ApplicationDbContext context, ILogger<FormModuleData> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ================================================
        // Métodos SQL
        // ================================================

        //Metodo para traer todo de FormModule SQL
        public async Task<IEnumerable<MostrarFormModuleDto>> GetAllAsync()
        {
            string query = @"SELECT fm.Id,
                         f.Name AS FormName,
                          m.Name AS ModuleName,
                            fm.Active,                            
                            fm.FormId,
                            fm.ModuleId

                            FROM FormModule fm
                            INNER JOIN Form f ON fm.FormId = f.Id
                            INNER JOIN Module m ON fm.ModuleId = m.Id
                            WHERE fm.Active = 1;";


            return await _context.QueryAsync<MostrarFormModuleDto>(query);
        }

        //Metodo para traer por id el FormModule SQL
        public async Task<MostrarFormModuleDto?> GetByIdAsync(int id)
        {
            try
            {
                string query = @"
                             SELECT fm.Id,
                            f.Name AS FormName,
                            m.Name AS ModuleName,
                            fm.Active,                            
                            fm.FormId,
                            fm.ModuleId

                            FROM FormModule fm
                            INNER JOIN Form f ON fm.FormId = f.Id
                            INNER JOIN Module m ON fm.ModuleId = m.Id
                            WHERE fm.Id = @Id;";
                return await _context.QueryFirstOrDefaultAsync<MostrarFormModuleDto>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al traer el FormModule por id {id}");
                throw;
            }

        }
        //Metodo para crear el FormModule SQL
        public async Task<FormModule> CreateAsync(FormModule formModule)
        {
            try
            {
                const string query = @"INSERT INTO [FormModule]
                                       (ModuleId ,FormId ,Active)
                                 VALUES
                                       (@ModuleId, @FormId, @Active)";

                formModule.Id = await _context.ExecuteScalarAsync<int>(query, new
                {
                    FormId = formModule.FormId,
                    ModuleId = formModule.ModuleId,
                    Active = true,
                });
                return formModule;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"no se pudo agregar persona {formModule}");
                throw;
            }
        }

        //Metodo para actualizar el FormModule SQL
        public async Task<bool> UpdateAsync(FormModule formModule)
        {
            if (formModule is null)
            {
                throw new ArgumentNullException(nameof(FormModule), "El formModule no puede ser nulo.");
            }
            try
            {
                const string query = @"UPDATE FormModule " +
                                      " SET FormId = @FormId, ModuleId = @ModuleID " +
                                      "WHERE Id = @Id ";
                var parameters = new { 
                    formModule.Id,
                    formModule.FormId, 
                    formModule.ModuleId 
                };
                int rowsAffected = await _context.ExecuteAsync(query, parameters);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"No se pudo actualizar {formModule}");
                throw;

            }

        }
        //Metodo para eleminar FormModule de manera Logico SQL
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"
                    UPDATE FormModule 
                    SET Active = 0
                    WHERE Id = @Id";

                int rowsAffected = await _context.ExecuteAsync(query, new { Id = id });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al borra logicamente con ID {FormModuleId}");
                throw;
            }
        }
        //Metodo para eliminar FormModule de manera persistente SQL
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = @"
                    DELETE FROM FormModule 
                    WHERE Id = @Id";

                int rowsAffected = await _context.ExecuteAsync(query, new { Id = id });

                return rowsAffected > 0; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente FormModule con ID {FormModuleId}", id);
                throw;
            }
        }

        // ================================================
        // Métodos LINQ
        // ================================================

        //Metodo para traer todo de FormModule LinQ
        public async Task<IEnumerable<FormModule>> GetAllLinQAsync()
        {
            try
            {
                return await _context.Set<FormModule>()
                        .Include(fm => fm.Form)   
                        .Include(fm => fm.Module) 
                        .Where(fm => !fm.Active)
                        .ToListAsync(); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al traer el FormModule ");
                throw;
            }


        }

        //Metodo para traer por id de FormMoudle LinQ
        public async Task<FormModule?> GetByIdLinQAsync(int id)
        {
            try
            {
                return await _context.Set<FormModule>()
                    .Include(fm => fm.Form)
                    .Include(fm => fm.Module)
                    .FirstOrDefaultAsync(fm => fm.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al traer el FormModule por id {id}");
                throw;
            }

        }

        //Metodo para crear FormModule LinQ
        public async Task<FormModule> CreateLinQAsync(FormModule formModule)
        {
            try
            {
                formModule.Active = formModule.Active ? formModule.Active : true;
                await _context.Set<FormModule>().AddAsync(formModule);
                await _context.SaveChangesAsync();
                return formModule;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"no se pudo agregar persona {formModule}");
                throw;
            }

        }

        //Metodo para actualizar FormModule LinQ
        public async Task<bool> UpdateLinQAsync(FormModule formModule)
        {
            if (formModule is null)
            {
                throw new ArgumentNullException(nameof(FormModule), "El formModule no puede ser nulo.");
            }
            try
            {
                _context.Set<FormModule>().Update(formModule);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"No se pudo actualizar {formModule}");
                throw;

            }

        }


        //Metodo para eliminador logico de FormModule LinQ
        public async Task<bool> DeleteLinQAsync(int id)
        {
            try
            {
                var entity = await _context.Set<FormModule>().FindAsync(id);
                if (entity == null) return false;

                // Marcar como eliminado
                entity.Active = true;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error al realizar delete lógico con LINQ: {ex.Message}");
                return false;
            }

        }

        //Metodo para eliminador persistente de FormModule LinQ

        public async Task<bool> DeletePersistenceLinQAsync(int id)
        {
            try
            {
                const string query = "UPDATE FormModule " +
                                     "SET IsDeleted = 1 " +
                                     "WHERE Id = @Id";
                var parameters = new { Id = id };
                await _context.ExecuteAsync(query, parameters);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error al realizar delete lógico: {ex.Message}");
                return false;
            }

        }


    }
}
