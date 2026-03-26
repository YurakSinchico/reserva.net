using API_Consumer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Reserva.API.Data;
using Reserva.Modelos;

namespace Reservas.MVC.Controllers
{
    public class ReservasController : Controller
    {
        

        // 2. MÉTODOS AUXILIARES PARA CARGAR DROPDOWNS (Como GetClientes/GetLibros)
        private List<SelectListItem> GetCanchas()
        {
            var canchas = Crud<Canchas>.GetAll();
            return canchas.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.nombre_cancha
            }).ToList();
        }

        private List<SelectListItem> GetHorarios()
        {
            var horarios = Crud<Horarios>.GetAll();
            return horarios.Select(h => new SelectListItem
            {
                Value = h.Id.ToString(),
                Text = $"{h.hora_inicio:hh\\:mm} - {h.hora_fin:hh\\:mm}"
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



        // GET: ReservasController
        public ActionResult Index()
        {
            var reservas = Crud<Reserva.Modelos.Reservas>.GetAll();
            return View(reservas);
        }

        // GET: ReservasController/Details/5
        public ActionResult Details(int id)
        {
            
            return View();
        }

        // GET: Reservas/Create
        public ActionResult Create(int? canchaId, DateTime? fecha)
        {
            ViewBag.Canchas = GetCanchas();
            ViewBag.CanchaPreseleccionada = canchaId;

            if (canchaId.HasValue && fecha.HasValue)
            {
                var todos = Crud<Horarios>.GetAll();
                var ocupados = Crud<Reserva.Modelos.Reservas>.GetAll()
                    .Where(r => r.CanchasId == canchaId && r.fecha_reserva.Date == fecha.Value.Date)
                    .Select(r => r.HorariosId).ToList();

                ViewBag.Horarios = todos.Where(h => !ocupados.Contains(h.Id)).ToList();
            }
            else { ViewBag.Horarios = new List<Horarios>(); }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Reserva.Modelos.Reservas reserva)
        {
            try
            {
                // 1. Identificar al usuario (Wally es ID 3)
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                reserva.ClientesId = !string.IsNullOrEmpty(userIdClaim) ? int.Parse(userIdClaim) : 3;

                // 2. Limpieza para la API de Render
                reserva.Clientes = null;
                reserva.Canchas = null;
                reserva.Horarios = null;

                // 3. Formato de fecha compatible con PostgreSQL
                reserva.fecha_reserva = DateTime.SpecifyKind(reserva.fecha_reserva.Date, DateTimeKind.Utc);

                // 4. Ejecutar creación
                API_Consumer.Crud<Reserva.Modelos.Reservas>.Create(reserva);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error: " + ex.Message);
                ViewBag.Canchas = GetCanchas();
                ViewBag.Horarios = new List<Horarios>();
                return View(reserva);
            }
        }

        // Método auxiliar para no repetir código de recarga
        private ActionResult ReloadView(Reserva.Modelos.Reservas reserva)
        {
            ViewBag.Canchas = GetCanchas();
            ViewBag.Horarios = new List<Horarios>();
            return View(reserva);
        }

        // GET: ReservasController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ReservasController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ReservasController/Delete/5
        public ActionResult Delete(int id)
        {
            var reserva = Crud<Reserva.Modelos.Reservas>.GetById(id);
            if (reserva == null)
            {
                return NotFound();
            }
            return View(reserva);
        }

        // POST: ReservasController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Reserva.Modelos.Reservas reserva)
        {
            try
            {
                Crud<Reserva.Modelos.Reservas>.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex) 
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }
    }
}
