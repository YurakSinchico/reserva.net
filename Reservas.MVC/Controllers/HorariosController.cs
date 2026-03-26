using API_Consumer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reserva.Modelos;

namespace Reservas.MVC.Controllers
{
    public class HorariosController : Controller
    {
        // GET: HorariosController
        public ActionResult Index()
        {
            // Forzamos que use el modelo que la vista espera
            List<Reserva.Modelos.Horarios> horarios = Crud<Reserva.Modelos.Horarios>.GetAll();
            return View(horarios);
        }

        // GET: HorariosController/Details/5
        public ActionResult Details(int id)
        {
            var horarios =Crud<Horarios>.GetById(id);
            if (horarios == null)
            {
                return NotFound();
            }
            return View(horarios);
        }

        // GET: HorariosController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HorariosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                var nuevoHorario = new Reserva.Modelos.Horarios
                {
                    // Parseamos el string del formulario directamente a TimeOnly
                    hora_inicio = TimeOnly.Parse(collection["hora_inicio"]),
                    hora_fin = TimeOnly.Parse(collection["hora_fin"]),
                    dia = collection["dia"] // Enviamos el valor del texto
                };

                API_Consumer.Crud<Reserva.Modelos.Horarios>.Create(nuevoHorario);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Verifica que los campos estén llenos: " + ex.Message);
                return View();
            }
        }
        // GET: HorariosController/Edit/5
        public ActionResult Edit(int id)
        {
            var horario = Crud<Horarios>.GetById(id);
            if (horario == null)
            {
                return NotFound();
            }
            return View(horario);
        }

        // POST: HorariosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Horarios horario)
        {
            try
            {
                // Mantenemos tu lógica de actualización
                Crud<Horarios>.Update(id, horario);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        // GET: HorariosController/Delete/5
        public ActionResult Delete(int id)
        {
            var horario = Crud<Horarios>.GetById(id);
            if (horario == null)
            {
                return NotFound();
            }
            return View(horario);
        }

        // POST: HorariosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Horarios horario)
        {
            try
            {
                // Mantenemos tu lógica de eliminación
                Crud<Horarios>.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }
    }
}
