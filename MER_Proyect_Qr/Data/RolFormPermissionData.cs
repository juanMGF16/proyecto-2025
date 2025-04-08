
using Entity.Context;
using Entity.DTOs;
using Entity.DTOs.Mostrar;
using Entity.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Data
{
    public class RolFormPermissionData
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RolFormPermissionData> _logger;

        public RolFormPermissionData(ApplicationDbContext context, ILogger<RolFormPermissionData> logger)
        {
            _context = context;
            _logger = logger;
        }
        // ================================================
        // Métodos SQL
        // ================================================
        public async Task<IEnumerable<MostrarRolFormPermissionDto>> GetAllAsyncSQL()
        {
            try
            {
                string query = @"
                            SELECT 
                                RFP.Id, 
                                R.Name AS RolName, 
                                F.Name AS FormName, 
                                P.Name AS PermissionName, 
                                RFP.Active, 
                                RFP.RolId, 
                                RFP.FormId, 
                                RFP.PermissionId
                            FROM RolFormPermission RFP
                            INNER JOIN Rol R ON RFP.RolId = R.Id
                            INNER JOIN Form F ON RFP.FormId = F.Id
                            INNER JOIN Permission P ON RFP.PermissionId = P.Id
                            WHERE RFP.Active = 1;";
                return await _context.QueryAsync<MostrarRolFormPermissionDto>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los roles de usuario desde SQL.");
                throw;
            }
        }

        public async Task<MostrarRolFormPermissionDto?> GetByIdAsync(int id)
        {
            try
            {
                string Query = @"SELECT 
                                    RFP.Id, 
                                    R.Id AS RolId,
                                    R.Name AS RolName, 
                                    F.Id AS FormId,
                                    F.Name AS FormName, 
                                    P.Id AS PermissionId,
                                    P.Name AS PermissionName 
                                FROM RolFormPermission RFP
                                INNER JOIN Rol R ON RFP.RolId = R.Id
                                INNER JOIN Form F ON RFP.FormId = F.Id
                                INNER JOIN Permission P ON RFP.PermissionId = P.Id
                                WHERE RFP.Id = @Id";

                return await _context.QueryFirstOrDefaultAsync<MostrarRolFormPermissionDto>(Query, new { Id = id });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el permiso de rol con ID {RolFormPermissionId}", id);
                throw; // Relanza la excepcion  para q sea manejada por las capas superiores

            }
        }

        // Método para crear un RolFormPermission con SQL
        public async Task<RolFormPermission> CreateAsync(RolFormPermission rolFormPermission)
        {
            try
            {

                string Query = @"INSERT INTO RolFormPermission" +
                                " (RolId, FormId, PermissionId, Active)" +
                                " OUTPUT INSERTED.Id" +
                                " VALUES" +
                                " (@RolId, @FormId, @PermissionId, 1)";

                rolFormPermission.Id = await _context.ExecuteScalarAsync<int>(Query, new
                {
                    @RolId = rolFormPermission.RolId,
                    @FormId = rolFormPermission.@FormId,
                    @PermissionId = rolFormPermission.@PermissionId,
                });

                return rolFormPermission;
            
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"no se pudo agregar rolFormPermission {rolFormPermission}");
                throw;
            }
        }

        // Método para Actualizar un RolFormPermission con SQL
        public async Task<bool> UpdateAsync(RolFormPermission rolFormPermission)
        {
            try
            {
                string Query = @"UPDATE RolFormPermission
                             SET RolId = @RolId,
                                 FormId = @FormId,
                                 PermissionId = @PermissionId
                             WHERE Id = @Id";

                var Parameters = new
                {
                    rolFormPermission.Id,
                    rolFormPermission.RolId,
                    rolFormPermission.FormId,
                    rolFormPermission.PermissionId,
                };

                // Ejecutar la consulta y obtener el número de filas afectadas
                int rowsAffected = await _context.ExecuteAsync(Query, Parameters);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"No se pudo actualizar RolFormPermission con ID {rolFormPermission.Id}");
                throw;
            }
        }

        // Método para Eliminar un rolFormPermission Logico SQL
        public async Task<bool> DeleteLogicAsync(int id)
        {
            try
            {
                string Query = @"
                                UPDATE RolFormPermission
                                SET
                                    Active = 0
                                WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(Query, new { Id = id });
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al borra logicamente con ID {RolFOrmPermission}", id);
                throw;
            }
        }

        // Método para eleminar un rolFormPermission con persistencia SQL
        public async Task<bool> DeletePersistenceAsync(int id)
        {
            try
            {
                string Query = @"DELETE FROM RolFormPermission" +
                                " WHERE Id = @Id";
                int rowsAffected = await _context.ExecuteAsync(Query, new { Id = id });
                return rowsAffected > 0; // almenos una fila
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar permanentemente rolFormPermission con ID {Id}", id);
                throw;
            }
        }

        // ================================================
        // Métodos LINQ
        // ================================================

        // Obtner todos los RolFormPermision
        public async Task<IEnumerable<RolFormPermission>> GetAllAsync()
        {
            try
            {
                return await _context.Set<RolFormPermission>()
                .Include(rfp => rfp.Rol)
                .Include(rfp => rfp.Form)
                .Include(rfp => rfp.Permission)
                .Where(rfp => rfp.Active)
                .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los usuarios.");
                throw; 
            }
        }

        public async Task<RolFormPermission?> GetByIdLinQAsync(int id)
        {
            try
            {
                return await _context.Set<RolFormPermission>()
                                   .Include(rfp => rfp.Rol)
                                    .Include(rfp => rfp.Form)
                                    .Include(rfp => rfp.Permission)
                                    .FirstOrDefaultAsync(rfp => rfp.Id == id);
                                    
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el permiso de rol con ID {RolFormPermissionId}", id);
                throw; // Relanza la excepcion  para q sea manejada por las capas superiores

            }
        }


        // Crear RolFormPermission con LINQ
        public async Task<RolFormPermission> CreateLinQAsync(RolFormPermission rolFormPermission)
        {
            try
            {
                await _context.Set<RolFormPermission>().AddAsync(rolFormPermission);
                rolFormPermission.Active = true;
                await _context.SaveChangesAsync();
                return rolFormPermission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear RolFormPermission");
                throw;
            }
        }

        // Actualizar RolFormPermission
        public async Task<bool> UpdateLinQAsync(RolFormPermission rolFormPermission)
        {

            //if (rolFormPermission is null)
            //{
            //    throw new ArgumentNullException(nameof(RolFormPermission), "El formModule no puede ser nulo.");
            //}

            try
            {
                _context.Set<RolFormPermission>().Update(rolFormPermission);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar RolFormPermission");
                return false;
            }
        }

        // Eliminar RolUser Logico LINQ
        public async Task<bool> DeleteLoqicLinqAsinc(int id)
        {
            try
            {
                var rolFormPermission = await GetByIdLinQAsync(id);
                if (rolFormPermission == null)
                    return false;

                rolFormPermission.Active = false;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar lógicamente el user con ID {id}", id);
                throw;
            }
        }

        // Eliminar RolFormPermission con LINQ Persistente
        //public async Task<bool> DeleteAsync(int id)
        //{
        //    try
        //    {
        //        var rolFormPermission = await GetByIdAsync(id);
        //        if (rolFormPermission == null)
        //        {
        //            return false;
        //        }
        //        _context.Set<RolFormPermission>().Remove(rolFormPermission);
        //        await _context.SaveChangesAsync();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error al eliminar RolFormPermission con el ID {RolFormPermissionId}", id);
        //        return false;
        //    }
        //}
    }
}
