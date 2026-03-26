
using API_Consumer;
using Reserva.Modelos;
using Reservas.Servicios;
using Reservas.Servicios.Interfaces;

Crud<Clientes>.EndPoint = "https://reserva-net.onrender.com/api/Clientes";
Crud<Canchas>.EndPoint = "https://reserva-net.onrender.com/api/Canchas";
Crud<Tipo_Canchas>.EndPoint = "https://reserva-net.onrender.com/api/Tipo_Canchas";
Crud<Reserva.Modelos.Reservas>.EndPoint = "https://reserva-net.onrender.com/api/Reservas";
// Asegúrate de que esta línea esté antes del builder.Build()
API_Consumer.Crud<Reserva.Modelos.Horarios>.EndPoint = "https://reserva-net.onrender.com/api/Horarios";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IAuthService, AuthService>();


builder.Services.AddAuthentication("Cookies") //cokies
                .AddCookie("Cookies", options =>
                {
                    options.LoginPath = "/Account/Index"; // Ruta de inicio de sesión


                });
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Index}/{id?}");
app.Run();
