using Business;
using Data;
using Data.Factory;
using Data.Interfaces;
using Entity.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔹 CORS
var OrigenesPermitidos = builder.Configuration.GetValue<string>("OrigenesPermitidos")!.Split(",");
builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(politica =>
    {
        politica.WithOrigins(OrigenesPermitidos).AllowAnyHeader().AllowAnyMethod();
    });
});

// 🔹 Leer el proveedor desde appsettings.json
string dbProvider = builder.Configuration["DatabaseSettings:Provider"] ?? "SqlServer";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connString = builder.Configuration.GetConnectionString(dbProvider); // Usa el nombre dinámico
    if (dbProvider == "SqlServer")
        options.UseSqlServer(connString);
    else if (dbProvider == "PostgreSql")
        options.UseNpgsql(connString);
    else if (dbProvider == "MySql")
        options.UseMySQL(connString);
    else
        throw new NotSupportedException($"El proveedor de base de datos {dbProvider} no es soportado");
});


// 🔹 Repositorio dinámico para UserData usando Factory
builder.Services.AddScoped<IUserData>(sp =>
{
    var context = sp.GetRequiredService<ApplicationDbContext>();
    var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
    return UserDataFactory.Create(dbProvider, context, loggerFactory);
});

// 🔹 Registrar servicios de negocio y datos
builder.Services.AddScoped<UserBusiness>();
builder.Services.AddScoped<PersonData>();
builder.Services.AddScoped<PersonBusiness>();
builder.Services.AddScoped<FormData>();
builder.Services.AddScoped<FormBusiness>();
builder.Services.AddScoped<ModuleData>();
builder.Services.AddScoped<ModuleBusiness>();
builder.Services.AddScoped<PermissionData>();
builder.Services.AddScoped<PermissionBusiness>();
builder.Services.AddScoped<RolData>();
builder.Services.AddScoped<RolBusiness>();
builder.Services.AddScoped<FormModuleData>();
builder.Services.AddScoped<FormModuleBusiness>();
builder.Services.AddScoped<RolFormPermissionData>();
builder.Services.AddScoped<RolFormPermissionBusiness>();
builder.Services.AddScoped<RolUserData>();
builder.Services.AddScoped<RolUserBusiness>();

var app = builder.Build();

// 🔹 Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
