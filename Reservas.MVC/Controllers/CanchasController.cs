using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Reserva.Modelos;
using System.Net.Http.Json;
using System.Security.Claims;

namespace Reservas.MVC.Controllers
{
    public class CanchasController : Controller
    {
        private readonly HttpClient _http;
        private readonly string _adminEmail = "yurianrango3@gmail.com";

        public CanchasController(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient();
            // Asegúrate de que la URL termine en / para que los endpoints se concatenen bien
            _http.BaseAddress = new Uri("https://reserva-net.onrender.com/api/");
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var canchas = await _http.GetFromJsonAsync<List<Canchas>>("Canchas") ?? new List<Canchas>();
                return View(canchas);
            }
            catch
            {
                return View(new List<Canchas>());
            }
        }

        public async Task<IActionResult> Admin()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (User.Identity.IsAuthenticated && userEmail == _adminEmail)
            {
                try
                {
                    var canchas = await _http.GetFromJsonAsync<List<Canchas>>("Canchas") ?? new List<Canchas>();
                    return View(canchas);
                }
                catch
                {
                    return View(new List<Canchas>());
                }
            }
            return RedirectToAction("Index");
        }

        // GET: Canchas/Create
        public async Task<IActionResult> Create()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (User.Identity.IsAuthenticated && userEmail == _adminEmail)
            {
                try
                {
                    // ✅ CORRECCIÓN: Usamos "Tipo_Canchas" con guion bajo como se ve en tu carpeta de Controllers
                    var tipos = await _http.GetFromJsonAsync<List<Tipo_Canchas>>("Tipo_Canchas") ?? new List<Tipo_Canchas>();
                    ViewBag.Tipos = new SelectList(tipos, "Id", "nombre_tip_cancha");
                }
                catch
                {
                    // Si falla la API, enviamos una lista vacía para que no de error la vista
                    ViewBag.Tipos = new SelectList(new List<Tipo_Canchas>(), "Id", "nombre_tip_cancha");
                }

                return View();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Create(Canchas cancha)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            if (User.Identity.IsAuthenticated && userEmail == _adminEmail)
            {
                // Enviamos los datos a la API
                var response = await _http.PostAsJsonAsync("Canchas", cancha);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Admin");
                }

                // Si algo falla al guardar, volvemos a cargar los tipos para mostrar el error en la misma vista
                var tipos = await _http.GetFromJsonAsync<List<Tipo_Canchas>>("Tipo_Canchas") ?? new List<Tipo_Canchas>();
                ViewBag.Tipos = new SelectList(tipos, "Id", "nombre_tip_cancha");
                return View(cancha);
            }
            return RedirectToAction("Index");
        }
        // GET: Canchas/Delete/5
        // Se usa para obtener los datos de la cancha antes de borrarla
        public async Task<IActionResult> Delete(int id)
        {
            // Buscamos la cancha específica en la API
            var cancha = await _http.GetFromJsonAsync<Canchas>($"Canchas/{id}");

            if (cancha == null)
            {
                return NotFound();
            }

            return View(cancha);
        }

        // POST: Canchas/Delete/5
       
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var response = await _http.DeleteAsync($"Canchas/{id}");

            if (response.IsSuccessStatusCode)
            {
                // Si se elimina con éxito, regresamos al panel de administración
                return RedirectToAction("Admin");
            }

            //mensjae de error de que no se puede eliminar
            ModelState.AddModelError("", "No se pudo eliminar la cancha. Inténtalo de nuevo.");
            return RedirectToAction("Admin");
        }
    }


}