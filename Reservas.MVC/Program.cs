using API_Consumer;
using Reserva.Modelos;
using Reservas.Servicios;
using Reservas.Servicios.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;

// Configuración de Endpoints de la API
Crud<Clientes>.EndPoint = "https://reserva-net.onrender.com/api/Clientes";
Crud<Canchas>.EndPoint = "https://reserva-net.onrender.com/api/Canchas";
Crud<Tipo_Canchas>.EndPoint = "https://reserva-net.onrender.com/api/Tipo_Canchas";
Crud<Reserva.Modelos.Reservas>.EndPoint = "https://reserva-net.onrender.com/api/Reservas";
API_Consumer.Crud<Reserva.Modelos.Horarios>.EndPoint = "https://reserva-net.onrender.com/api/Horarios";

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURACIÓN DE SERVICIOS (CONTENEDOR) ---
builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IAuthService, AuthService>();

// NUEVO: Configuración de Sesiones
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de expiración
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddAuthentication("Cookies")
                .AddCookie("Cookies", options =>
                {
                    options.LoginPath = "/Account/Index";
                });

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// --- 2. CONFIGURACIÓN DEL PIPELINE (EL ORDEN ES CRÍTICO) ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Importante para cargar tus estilos .css

app.UseRouting();

// NUEVO: Debe ir después de Routing y ANTES de Authentication/Authorization
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Index}/{id?}"); // este me manda al loging

app.Run();