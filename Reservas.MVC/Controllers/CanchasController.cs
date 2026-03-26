using API_Consumer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Reserva.Modelos;
using System.Security.Claims;

namespace Reservas.MVC.Controllers
{
    public class CanchasController : Controller
    {
        private readonly string _adminEmail = "yurianrango3@gmail.com";

        // Vista para Deportistas (Catálogo)
        public IActionResult Index()
        {
            var canchas = Crud<Canchas>.GetAll() ?? new List<Canchas>();
            // Solo mostramos las que están habilitadas (Estado: true)
            var canchasActivas = canchas.Where(c => c.estado_cancha).ToList();
            return View(canchasActivas);
        }

        // Panel Administrativo (Inventario)
        public IActionResult Admin()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (User.Identity.IsAuthenticated && userEmail == _adminEmail)
            {
                var canchas = Crud<Canchas>.GetAll() ?? new List<Canchas>();
                return View(canchas);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Create()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (userEmail == _adminEmail)
            {
                var tipos = Crud<Tipo_Canchas>.GetAll() ?? new List<Tipo_Canchas>();
                ViewBag.Tipos = new SelectList(tipos, "Id", "nombre_tip_cancha");
                return View();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Canchas cancha)
        {
            if (User.FindFirstValue(ClaimTypes.Email) == _adminEmail)
            {
                cancha.estado_cancha = true; // Por defecto activa al crear
                Crud<Canchas>.Create(cancha);
                return RedirectToAction("Admin");
            }
            return Forbid();
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (User.FindFirstValue(ClaimTypes.Email) == _adminEmail)
            {
                var cancha = Crud<Canchas>.GetById(id);
                if (cancha == null) return NotFound();

                var tipos = Crud<Tipo_Canchas>.GetAll() ?? new List<Tipo_Canchas>();
                ViewBag.Tipos = new SelectList(tipos, "Id", "nombre_tip_cancha", cancha.Tipo_CanchasId);
                return View(cancha);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Canchas cancha)
        {
            if (User.FindFirstValue(ClaimTypes.Email) == _adminEmail)
            {
                // La propiedad estado_cancha se actualiza desde el switch de la vista
                Crud<Canchas>.Update(id, cancha);
                return RedirectToAction("Admin");
            }
            return Forbid();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (User.FindFirstValue(ClaimTypes.Email) == _adminEmail)
            {
                Crud<Canchas>.Delete(id);
                return RedirectToAction("Admin");
            }
            return Forbid();
        }
    }
}