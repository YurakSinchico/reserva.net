using API_Consumer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reserva.Modelos;
using System;
using System.Collections.Generic;

namespace Reservas.MVC.Controllers
{
    public class HorariosController : Controller
    {
        // GET: Horarios
        public ActionResult Index()
        {
            // Mantenemos tu lógica de obtener la lista completa
            List<Reserva.Modelos.Horarios> horarios = Crud<Reserva.Modelos.Horarios>.GetAll() ?? new List<Horarios>();
            return View(horarios);
        }

        // GET: Horarios/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Horarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                var nuevoHorario = new Reserva.Modelos.Horarios
                {
                    // Parseo de seguridad para TimeOnly
                    hora_inicio = TimeOnly.Parse(collection["hora_inicio"]),
                    hora_fin = TimeOnly.Parse(collection["hora_fin"]),
                    dia = collection["dia"]
                };

                Crud<Reserva.Modelos.Horarios>.Create(nuevoHorario);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear: " + ex.Message);
                return View();
            }
        }

        // GET: Horarios/Edit/5
        public ActionResult Edit(int id)
        {
            var horario = Crud<Horarios>.GetById(id);
            if (horario == null) return NotFound();
            return View(horario);
        }

        // POST: Horarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Horarios horario)
        {
            try
            {
                Crud<Horarios>.Update(id, horario);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(horario);
            }
        }

        // POST: Horarios/Delete/5
        // Este es el método que llama tu botón "Eliminar" de la tabla
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                bool eliminado = Crud<Horarios>.Delete(id);

                if (eliminado)
                {
                    TempData["Mensaje"] = "Horario eliminado correctamente.";
                }
                else
                {
                    TempData["Error"] = "No se pudo eliminar el registro.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error de base de datos: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}