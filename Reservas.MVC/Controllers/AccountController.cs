using API_Consumer;
using Reserva.Modelos;
using Reservas.Servicios.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Reserva.Modelos;
using Reservas.Servicios.Interfaces;

namespace Reservas.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        // GET: Account
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            username = username.Trim().ToLower();

            if (await _authService.Login(username, password))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Email o contraseña incorrectos.";
                return View("Index");
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string nombre_cliente, string apellido_cliente, string correo_cliente, string contrasena_cliente,string telefono_cliente,DateOnly fecha_nacimiento_cliente)
        {
            correo_cliente = correo_cliente.Trim().ToLower();

            var usuario = Crud<Clientes>.GetAll()
                .FirstOrDefault(u => u.correo_cliente.ToLower() == correo_cliente);

            if (usuario != null)
            {
                ViewBag.ErrorMessage = "Esta cuenta ya está asociada a este correo";
                return View();
            }

            if (await _authService.Register(nombre_cliente, apellido_cliente, correo_cliente, contrasena_cliente, telefono_cliente,fecha_nacimiento_cliente))
            {
                return RedirectToAction("Index", "Account");
            }

            ViewBag.ErrorMessage = "Error al crear el usuario";
            return View();
        }


        public async Task<IActionResult> Logout()
        {
            // Elimina la cookie de autenticación
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Index", "Account");
        }


    }
}