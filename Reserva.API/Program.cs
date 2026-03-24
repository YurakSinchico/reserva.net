using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Reserva.API.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de la Base de Datos (PostgreSQL)
builder.Services.AddDbContext<ReservaAPIContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Reservas_Render")
    ?? throw new InvalidOperationException("Connection string 'API_Reservas' not found.")));

// 2. Registro de Servicios (Contenedor de Dependencias)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Swagger se registra aquí SIEMPRE
builder.Services.AddOpenApi();

var app = builder.Build();

// 3. Configuración del Pipeline de HTTP (Middleware)
if (app.Environment.IsDevelopment())
{
    // AQUÍ ES EL CAMBIO: Usamos 'app', no 'builder'
    app.UseSwagger();
    app.UseSwaggerUI(); // Esto habilita la interfaz visual en /swagger

    // Si usas .NET 9+, esto activa el nuevo OpenAPI
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();