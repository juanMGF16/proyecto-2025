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
    public class FormData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<FormData> _logger;

        public FormData(ApplicationDbContext context, ILogger<FormData> logger)
        {
            _context = context;
            _logger = logger;
        }   

        // ================================================
        // Métodos SQL
        // ================================================


        //Metodo para traer todo SQL
        public async Task<IEnumerable<Form>> GetAllAsync()
        {

            try
            {
                string query = @"SELECT 
                    Id, 
                    Name, 
                    Description, 
                    CreationDate, 
                    Active 
                FROM Form 
                WHERE Active = 1";

                return await _context.QueryAsync<Form>(query);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener Los form ");
                throw; // Relanza la excepcion  para q sea manejada por las capas superiores
            }
        }


        //Metodo para traer por id SQL
        public async Task<Form?> GetByIdAsync(int id)
        {
            try
            {
                string query = @"SELECT 
                    Id, 
                    Name, 
                    Description, 
                    CreationDate, 
                    Active 
                FROM Form 
                WHERE Id = @Id AND Active = 1";
                return await _context.QueryFirstOrDefaultAsync<Form>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el form con ID {FormId}", id);
                throw; // Relanza la excepcion  para q sea manejada por las capas superiores
            }
        }


        //Metodo para crear SQL
        public async Task<Form> CreateAsync(Form form)
        {
            try
            {
                string query = @"
                                INSERT INTO Form (Name, Description, CreationDate, Active) 
                                OUTPUT INSERTED.Id 
                                VALUES (@Name, @Description, @CreationDate, @Active);";

                form.Id = await _context.QueryFirstOrDefaultAsync<int>(query, new
                {
                    form.Name,
                    form.Description,
                    form.CreationDate,
                    form.Active
                });

                return form;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el formulario.");
                throw;
            }
        }

        //Metodo para actualizar SQL

        public async Task<bool> UpdateAsync(Form form)
        {
            try
            {
                string query = @"
                                UPDATE Form 
                                SET 
                                    Name = @Name, 
                                    Description = @Description, 
                                    CreationDate = @CreationDate, 
                                    Active = @Active 
                                WHERE Id = @Id";

                int rowsAffected = await _context.ExecuteAsync(query, new
                {
                    form.Id,
                    form.Name,
                    form.Description,
                    form.CreationDate,
                    form.Active
                });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el formulario con ID {FormId}", form.Id);
                throw;
            }
        }


        //Metodo para borrar logico SQL
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string query = @"
                            UPDATE Form
                            SET Active = 0
                            WHERE Id = @Id";

                int rowsAffected = await _context.ExecuteAsync(query, new { id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar logicamente form: {ex.Message}");
                return false;
            }
        }

        //Metodo para borrar persistente SQL
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string query = @"
                            DELETE FROM Form
                            WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(query, new { Id = id });

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar form: {ex.Message}");
                return false;
            }
        }


        // ================================================
        // Métodos LINQ
        // ================================================


        //Metodo para obtener todo de Formulario LinQ
        public async Task<IEnumerable<Form>> GetAllLinQAsync()
        {
            try
            {
                return await _context.Set<Form>().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "error al obtener form");
                throw;
            }
        }


        //Metodo para obtener por el formulario por id LinQ

        public async Task<Form?> GetByIdLinQAsync(int id)
        {
            try
            {
                return await _context.Set<Form>().FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la forma con ID {FormId}", id);
                throw; // Relanza la excepcion  para q sea manejada por las capas superiores
            }
        }

        //Metodo para crear el formulario LinQ
        public async Task<Form> CreateLinQAsync(Form form)
        {
            try
            {
                await _context.Set<Form>().AddAsync(form);
                await _context.SaveChangesAsync();
                return form;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear la forma: {ex.Message}");
                throw;
            }
        }

        //Metodo para actualizar el formulario LinQ
        public async Task<bool> UpdateLinQAsync(Form form)
        {
            try
            {
                _context.Set<Form>().Update(form);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al actualizar la forma: {ex.Message}");
                throw;
            }
        }


        //Metodo para eliminar un formulario de manera logico LinQ
        public async Task<bool> DeleteLogicLinQAsync(int id)
        {
            try
            {
                var form = await GetByIdLinQAsync(id);
                if (form == null)
                {
                    return false;
                }

                // Marcar como eliminado lógicamente
                form.Active = false; // O form.Active = 0; si es un campo numérico
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el form con ID {id}", id);
                throw;
            }
        }

        //Metodo para eliminar un formulario de manera persistente LinQ
        public async Task<bool> DeletePersistenceLinQAsync(int id)
        {
            try
            {
                var form = await GetByIdLinQAsync(id);
                if (form == null)
                {
                    return false;
                }
                _context.Set<Form>().Remove(form);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al eliminar la forma: {ex.Message}");
                throw;
            }
        }


    }
}
