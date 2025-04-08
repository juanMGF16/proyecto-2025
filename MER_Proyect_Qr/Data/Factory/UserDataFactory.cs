using Data.DbMotor;
using Data.Interfaces;
using Entity.Context;
using Entity.DTOs.Mostrar;
using Entity.Model;
using Microsoft.Extensions.Logging;

namespace Data.Factory
{
    public class UserDataFactory
    {
        public static IUserData<User, MostrarUserDto> Create(string dbType, ApplicationDbContext context, ILoggerFactory loggerFactory)
        {
            return dbType switch
            {
                "SqlServer" => new UserDataSqlServer(context, loggerFactory.CreateLogger<UserDataSqlServer>()),
                "PostgreSql" => new UserDataPostgres(context, loggerFactory.CreateLogger<UserDataPostgres>()),
                "MySql" => new UserDataMySql(context, loggerFactory.CreateLogger<UserDataMySql>()),
                _ => throw new NotSupportedException($"El tipo de base de datos {dbType} no es soportado")
            };
        }
    }
}
