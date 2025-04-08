using Business;
using Data;
using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<PersonData>();  // Asegúrate de que PersonaData está registrado
builder.Services.AddScoped<PersonBusiness>(); // Registra la capa de negocio
                                              // Si usa una interfaz
builder.Services.AddScoped<FormData>();
builder.Services.AddScoped<FormBusiness>();

builder.Services.AddScoped<ModuleData>();
builder.Services.AddScoped<ModuleBusiness>();

builder.Services.AddScoped<PermissionData>();
builder.Services.AddScoped<PermissionBusiness>();

builder.Services.AddScoped<RolData>();
builder.Services.AddScoped<RolBusiness>();

builder.Services.AddScoped<UserData>();
builder.Services.AddScoped<UserBusiness>();

builder.Services.AddScoped<FormModuleData>();
builder.Services.AddScoped<FormModuleBusiness>();

builder.Services.AddScoped<RolFormPermissionData>();
builder.Services.AddScoped<RolFormPermissionBusiness>();

builder.Services.AddScoped<RolUserData>();
builder.Services.AddScoped<RolUserBusiness>();



var OrigenesPermitidos = builder.Configuration.GetValue<string>("OrigenesPermitidos")!.Split(",");

builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(politica =>
    {
        politica.WithOrigins(OrigenesPermitidos).AllowAnyHeader().AllowAnyMethod();
    });
});
builder.Services.AddDbContext<ApplicationDbContext>(opciones => opciones
.UseSqlServer("name=DefaultConnection"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
