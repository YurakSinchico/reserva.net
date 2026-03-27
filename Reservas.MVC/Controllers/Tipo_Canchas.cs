using API_Consumer;
using Microsoft.AspNetCore.Mvc;
using Reserva.Modelos; 
using System.Net.Http.Json;

namespace Reservas.MVC.Controllers
{
   
    public class TipoCanchasController : Controller
    {
        private readonly HttpClient _http;

        public TipoCanchasController(IHttpClientFactory httpClientFactory)
        {
            _http = httpClientFactory.CreateClient();
            _http.BaseAddress = new Uri("https://reserva-net.onrender.com/api/");
        }

        // GET: /TipoCanchas/Index
        public async Task<IActionResult> Index()
        {
            try
            {
                var tipos = await _http.GetFromJsonAsync<List<Tipo_Canchas>>("Tipo_Canchas") ?? new List<Tipo_Canchas>();
                return View(tipos);
            }
            catch
            {
                return View(new List<Tipo_Canchas>());
            }
        }

        // GET: /TipoCanchas/Create
        public IActionResult Create()
        {
          
            return View();
        }

        // POST: /TipoCanchas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tipo_Canchas tipo)
        {
            if (ModelState.IsValid)
            {
                var response = await _http.PostAsJsonAsync("Tipo_Canchas", tipo);

                if (response.IsSuccessStatusCode)
                {
                    
                    return RedirectToAction("Create", "Canchas");
                }
            }

            
            return View(tipo);
        }
    }

}
