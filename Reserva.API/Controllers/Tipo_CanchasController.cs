using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reserva.API.Data;
using Reserva.Modelos;

namespace Reserva.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Tipo_CanchasController : ControllerBase
    {
        private readonly ReservaAPIContext _context;

        public Tipo_CanchasController(ReservaAPIContext context)
        {
            _context = context;
        }

        // GET: api/Tipo_Canchas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tipo_Canchas>>> GetTipo_Canchas()
        {
            return await _context.Tipo_Canchas.ToListAsync();
        }

        // GET: api/Tipo_Canchas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tipo_Canchas>> GetTipo_Canchas(int id)
        {
            var tipo_Canchas = await _context.Tipo_Canchas.FindAsync(id);

            if (tipo_Canchas == null)
            {
                return NotFound();
            }

            return tipo_Canchas;
        }

        // PUT: api/Tipo_Canchas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTipo_Canchas(int id, Tipo_Canchas tipo_Canchas)
        {
            if (id != tipo_Canchas.Id)
            {
                return BadRequest();
            }

            _context.Entry(tipo_Canchas).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Tipo_CanchasExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Tipo_Canchas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Tipo_Canchas>> PostTipo_Canchas(Tipo_Canchas tipo_Canchas)
        {
            _context.Tipo_Canchas.Add(tipo_Canchas);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTipo_Canchas", new { id = tipo_Canchas.Id }, tipo_Canchas);
        }

        // DELETE: api/Tipo_Canchas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipo_Canchas(int id)
        {
            var tipo_Canchas = await _context.Tipo_Canchas.FindAsync(id);
            if (tipo_Canchas == null)
            {
                return NotFound();
            }

            _context.Tipo_Canchas.Remove(tipo_Canchas);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool Tipo_CanchasExists(int id)
        {
            return _context.Tipo_Canchas.Any(e => e.Id == id);
        }
    }
}
