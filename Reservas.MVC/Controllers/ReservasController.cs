using API_Consumer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Reserva.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Reservas.MVC.Controllers
{
    public class ReservasController : Controller
    {
        // Eliminamos el constructor con _context porque usas Crud<T> (API)

        private void CargarListas(int? canchaId = null, DateTime? fecha = null)
        {
            // 1. Traemos todas las canchas para el desplegable
            var canchas = Crud<Canchas>.GetAll() ?? new List<Canchas>();
            ViewBag.Canchas = new SelectList(canchas, "Id", "nombre_cancha", canchaId);

            // 2. Traemos todos los horarios disponibles en el sistema
            var todosHorarios = Crud<Horarios>.GetAll() ?? new List<Horarios>();

            if (canchaId.HasValue)
            {
                // 3. Buscamos todas las reservas que ya existen para esa cancha y esa fecha
                DateTime fechaBusqueda = (fecha ?? DateTime.Now).Date;
                var todasReservas = Crud<Reserva.Modelos.Reservas>.GetAll() ?? new List<Reserva.Modelos.Reservas>();

                // 4. Obtenemos solo los IDs de los horarios que ya están ocupados
                var ocupadosIds = todasReservas
                    .Where(r => r.CanchasId == canchaId.Value &&
                                r.fecha_reserva.Date == fechaBusqueda)
                    .Select(r => r.HorariosId)
                    .ToList();

                // 5. FILTRO CLAVE: Solo enviamos a la vista los horarios cuyo ID NO esté en la lista de ocupados
                ViewBag.Horarios = todosHorarios
                    .Where(h => !ocupadosIds.Contains(h.Id))
                    .ToList();

                ViewBag.CanchaPreseleccionada = canchaId.Value;
            }
            else
            {
                // Si no hay cancha seleccionada, la lista de horarios empieza vacía
                ViewBag.Horarios = new List<Horarios>();
            }
        }

        // --- HISTORIAL (UNIFICADO) ---
        [HttpGet]
        public ActionResult Index()
        {
            // 1. Obtener datos de sesión
            var correoUsuario = HttpContext.Session.GetString("UsuarioCorreo")?.ToLower().Trim();
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            // 2. Traer datos de la API
            var listaReservas = Crud<Reserva.Modelos.Reservas>.GetAll() ?? new List<Reserva.Modelos.Reservas>();
            var canchas = Crud<Reserva.Modelos.Canchas>.GetAll() ?? new List<Canchas>();
            var horarios = Crud<Reserva.Modelos.Horarios>.GetAll() ?? new List<Horarios>();
            var clientes = Crud<Reserva.Modelos.Clientes>.GetAll() ?? new List<Clientes>();

            // 3. Cruzar datos (Llenar objetos Canchas, Horarios y Clientes)
            foreach (var r in listaReservas)
            {
                r.fecha_reserva = r.fecha_reserva.AddHours(12).Date;
                r.Canchas = canchas.FirstOrDefault(c => c.Id == r.CanchasId);
                r.Horarios = horarios.FirstOrDefault(h => h.Id == r.HorariosId);
                r.Clientes = clientes.FirstOrDefault(cl => cl.Id == r.ClientesId);
            }

            // 4. LÓGICA DE VISIBILIDAD
            // Definimos tu correo exacto de admin
            string miCorreoAdmin = "yurianrango3@gmail.com";

            // Si NO es el admin, aplicamos el filtro restrictivo
            if (correoUsuario != miCorreoAdmin)
            {
                if (usuarioId.HasValue)
                {
                    // Alberto o Wally solo ven lo que coincida con su ID
                    listaReservas = listaReservas.Where(r => r.ClientesId == usuarioId.Value).ToList();
                }
                else
                {
                    // Si no hay sesión, lista vacía por seguridad
                    listaReservas = new List<Reserva.Modelos.Reservas>();
                }
            }
            // SI ERES EL ADMIN: El código ignora el 'if' y muestra la lista completa tal cual viene de la DB.

            ViewBag.Canchas = canchas;
            ViewBag.Horarios = horarios;
            ViewBag.Clientes = clientes;

            return View(listaReservas.OrderByDescending(r => r.Id).ToList());
        }
        // --- CREAR (VISTA) ---
        [HttpGet]
        public ActionResult Create(int? canchaId, DateTime? fecha)
        {
            DateTime fechaSeleccionada = fecha?.Date ?? DateTime.Now.Date;

            // Esto limpia la lista de horarios ocupados antes de mostrar la página
            CargarListas(canchaId, fechaSeleccionada);

            var modelo = new Reserva.Modelos.Reservas
            {
                fecha_reserva = fechaSeleccionada,
                CanchasId = canchaId ?? 0
            };
            return View(modelo);
        }

        // --- CREAR (PROCESAR) ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Reserva.Modelos.Reservas reserva, string enviar)
        {
            if (enviar == "Ver turnos")
            {
                CargarListas(reserva.CanchasId, reserva.fecha_reserva);
                return View(reserva);
            }

            try
            {
                DateTime fechaSeleccionada = reserva.fecha_reserva.Date;
                reserva.fecha_reserva = DateTime.SpecifyKind(fechaSeleccionada, DateTimeKind.Unspecified);

                // Limpiar objetos de navegación para que la API no falle al serializar
                reserva.Clientes = null;
                reserva.Canchas = null;
                reserva.Horarios = null;

                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
                reserva.ClientesId = !string.IsNullOrEmpty(userIdClaim) ? int.Parse(userIdClaim) : (HttpContext.Session.GetInt32("UsuarioId") ?? 2);

                Crud<Reserva.Modelos.Reservas>.Create(reserva);

                TempData["Mensaje"] = "Reserva guardada con éxito.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al guardar: " + ex.Message);
                CargarListas(reserva.CanchasId, reserva.fecha_reserva);
                return View(reserva);
            }
        }

        // --- CANCELAR ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancelar(int id)
        {
            try
            {
                Crud<Reserva.Modelos.Reservas>.Delete(id);
                TempData["Mensaje"] = "Reserva cancelada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cancelar: " + ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}