using API_Consumer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Reserva.Modelos;
using System.Linq;
using System.Security.Claims;

namespace Reservas.MVC.Controllers
{
    public class ReservasController : Controller
    {
        private readonly string AdminEmail = "yurianrango3@gmail.com";

        // Obtener lista de canchas para los dropdowns
        private List<SelectListItem> GetCanchas()
        {
            var canchas = Crud<Canchas>.GetAll() ?? new List<Canchas>();
            return canchas.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.nombre_cancha
            }).ToList();
        }

        [HttpGet]
        public JsonResult GetHorariosDisponibles(int canchaId, DateTime fecha)
        {
            var todasLasReservas = Crud<Reserva.Modelos.Reservas>.GetAll() ?? new List<Reserva.Modelos.Reservas>();
            var todosLosHorarios = Crud<Horarios>.GetAll() ?? new List<Horarios>();

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

        // Listado de reservas (Admin ve todo, Cliente solo lo suyo)
        public ActionResult Index()
        {
            var todasLasReservas = Crud<Reserva.Modelos.Reservas>.GetAll() ?? new List<Reserva.Modelos.Reservas>();
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (userEmail == AdminEmail)
            {
                return View(todasLasReservas);
            }
            else
            {
                var misReservas = todasLasReservas
                    .Where(r => r.Clientes?.correo_cliente == userEmail)
                    .ToList();
                return View(misReservas);
            }
        }

        [HttpGet]
        public ActionResult Create(int? canchaId, DateTime? fecha)
        {
            // 1. Cargamos todas las canchas para el dropdown
            var listaCanchas = Crud<Canchas>.GetAll() ?? new List<Canchas>();
            ViewBag.Canchas = new SelectList(listaCanchas, "Id", "nombre_cancha");

            // 2. Preparamos el modelo
            var modelo = new Reserva.Modelos.Reservas();
            modelo.fecha_reserva = fecha ?? DateTime.Now;

            if (canchaId.HasValue)
            {
                modelo.CanchasId = canchaId.Value;
                // Guardamos el ID para que la vista sepa cuál marcar como "selected"
                ViewBag.CanchaPreseleccionada = canchaId.Value;

                // 3. Cargamos los horarios disponibles para ESA cancha y ESA fecha
                var todasReservas = Crud<Reserva.Modelos.Reservas>.GetAll() ?? new List<Reserva.Modelos.Reservas>();
                var todosHorarios = Crud<Horarios>.GetAll() ?? new List<Horarios>();

                var ocupadosIds = todasReservas
                    .Where(r => r.CanchasId == canchaId && r.fecha_reserva.Date == modelo.fecha_reserva.Date)
                    .Select(r => r.HorariosId)
                    .ToList();

                // Enviamos solo los que no están ocupados
                ViewBag.Horarios = todosHorarios.Where(h => !ocupadosIds.Contains(h.Id)).ToList();
            }
            else
            {
                ViewBag.Horarios = new List<Horarios>();
            }

            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Reserva.Modelos.Reservas reserva)
        {
            try
            {
                // Limpieza de objetos relacionados para evitar errores en el POST
                reserva.Clientes = null;
                reserva.Canchas = null;
                reserva.Horarios = null;

                // Formato de fecha para PostgreSQL
                reserva.fecha_reserva = DateTime.SpecifyKind(reserva.fecha_reserva.Date, DateTimeKind.Utc);

                // Asignar el ID del cliente logueado
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                reserva.ClientesId = !string.IsNullOrEmpty(userIdClaim) ? int.Parse(userIdClaim) : 1;

                Crud<Reserva.Modelos.Reservas>.Create(reserva);

                TempData["FechaExito"] = reserva.fecha_reserva.ToString("dd/MM/yyyy");
                return RedirectToAction("Success");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear reserva: " + ex.Message);
                return ReloadView(reserva);
            }
        }

        public IActionResult Success()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancelar(int id)
        {
            try
            {
                // 1. Buscamos la reserva por ID
                var reservaa = Crud<Reserva.Modelos.Reservas>.GetById(id);

                if (reservaa != null)
                {
                    // 2. Validación de fecha: solo cancelar si es hoy o futuro
                    if (reservaa.fecha_reserva.Date < DateTime.Now.Date)
                    {
                        TempData["Error"] = "No puedes cancelar reservas de fechas pasadas.";
                        return RedirectToAction(nameof(Index));
                    }

                    // 3. CORRECCIÓN ERROR CS1503: Pasamos el objeto si tu Crud lo requiere 
                    // o reservaa.Id si solo requiere el entero.
                    // Cambia 'reservaa' por 'reservaa.Id'
                    bool eliminado = Crud<Reserva.Modelos.Reservas>.Delete(reservaa.Id);

                    if (eliminado)
                    {
                        TempData["Mensaje"] = "Reserva cancelada y horario liberado.";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cancelar: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private ActionResult ReloadView(Reserva.Modelos.Reservas reserva)
        {
            ViewBag.Canchas = GetCanchas();
            return View(reserva);
        }
    }
}
