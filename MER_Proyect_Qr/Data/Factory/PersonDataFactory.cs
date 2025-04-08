using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.DbMotor;
using Data.Interfaces;
using Entity.Context;
using Entity.DTOs.Mostrar;
using Entity.Model;
using Microsoft.Extensions.Logging;

namespace Data.Factory
{
    public class PersonDataFactory
    {
        public static IUserData<Person, MostrarPersonDto> Create(string dbType, ApplicationDbContext context, ILoggerFactory loggerFactory)
        {
            return dbType switch
            {
                "SqlServer" => new PersonDataSqlServer(context, loggerFactory.CreateLogger<PersonDataSqlServer>()),
                //"PostgreSql" => new UserDataPostgres(context, loggerFactory.CreateLogger<UserDataPostgres>()),
                //"MySql" => new UserDataMySql(context, loggerFactory.CreateLogger<UserDataMySql>()),
                _ => throw new NotSupportedException($"El tipo de base de datos {dbType} no es soportado")
            };
        }
    }
}
