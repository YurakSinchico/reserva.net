using API_Consumer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Reserva.API.Data;
using Reserva.Modelos;
using System.Security.Claims;

namespace Reservas.MVC.Controllers
{
    public class ReservasController : Controller
    {
      
        private readonly string AdminEmail = "yurianrango3@gmail.com";

      
        private List<SelectListItem> GetCanchas()
        {
            var canchas = Crud<Canchas>.GetAll();
            return canchas.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.nombre_cancha
            }).ToList();
        }

        [HttpGet]
        public JsonResult GetHorariosDisponibles(int canchaId, DateTime fecha)
        {
            var todasLasReservas = Crud<Reserva.Modelos.Reservas>.GetAll();
            var todosLosHorarios = Crud<Horarios>.GetAll();

            var ocupadosIds = todasLasReservas
                .Where(r => r.CanchasId == canchaId && r.fecha_reserva.Date == fecha.Date)
                .Select(r => r.HorariosId)
                .ToList();

            var disponibles = todosLosHorarios
                .Where(h => !ocupadosIds.Contains(h.Id))
                .Select(h => new {
                    id = h.Id,
                    texto = $"{h.hora_inicio:hh\\:mm} - {h.hora_fin:hh\\:mm}"
                })
                .ToList();

            return Json(disponibles);
        }

        public ActionResult Index()
        {
            // Traemos la lista completa desde la API
            var todasLasReservas = Crud<Reserva.Modelos.Reservas>.GetAll() ?? new List<Reserva.Modelos.Reservas>();

            // Obtenemos el correo del usuario actual
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            // Si eres Yurak (admin), enviamos la lista completa
            if (userEmail == "yurianrango3@gmail.com")
            {
                return View(todasLasReservas);
            }
            else
            {
                // Si es un cliente como Wally, filtramos para que vea solo lo suyo
                var misReservas = todasLasReservas
                    .Where(r => r.Clientes?.correo_cliente == userEmail)
                    .ToList();

                return View(misReservas);
            }
        }

        [HttpGet]
        public ActionResult Create(int? canchaId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            // Verificamos si eres tú (Yurak)
            if (userEmail != "yurianrango3@gmail.com")
            {
                TempData["ErrorPermiso"] = "Solo la administradora puede crear reservas.";
                return RedirectToAction("Index");
            }

            ViewBag.Canchas = GetCanchas();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Reserva.Modelos.Reservas reserva)
        {
            // Seguridad en el envío de datos
            if (User.Identity?.Name != AdminEmail) return Forbid();

            try
            {
                // Limpieza para PostgreSQL en Render
                reserva.Clientes = null;
                reserva.Canchas = null;
                reserva.Horarios = null;
                reserva.fecha_reserva = DateTime.SpecifyKind(reserva.fecha_reserva.Date, DateTimeKind.Utc);

                // ID de usuario (Yurak)
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                reserva.ClientesId = !string.IsNullOrEmpty(userIdClaim) ? int.Parse(userIdClaim) : 1;

                API_Consumer.Crud<Reserva.Modelos.Reservas>.Create(reserva);

                // Pasamos la fecha a Success sin usar el ID para evitar el 404
                TempData["FechaExito"] = reserva.fecha_reserva.ToString("dd/MM/yyyy");
                return RedirectToAction("Success");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
                return ReloadView(reserva);
            }
        }

        public IActionResult Success()
        {
           
            return View();
        }

        public ActionResult Delete(int id)
        {
            if (User.Identity?.Name != AdminEmail) return RedirectToAction("Index");

            var reserva = Crud<Reserva.Modelos.Reservas>.GetById(id);
            if (reserva == null) return NotFound();
            return View(reserva);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Reserva.Modelos.Reservas reserva)
        {
            if (User.Identity?.Name != AdminEmail) return Forbid();

            try
            {
                Crud<Reserva.Modelos.Reservas>.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "No se pudo eliminar: " + ex.Message);
                return View();
            }
        }

        private ActionResult ReloadView(Reserva.Modelos.Reservas reserva)
        {
            ViewBag.Canchas = GetCanchas();
            ViewBag.Horarios = new List<Horarios>();
            return View(reserva);
        }
    }
}
