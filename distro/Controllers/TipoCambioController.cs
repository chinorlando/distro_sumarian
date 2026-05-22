using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OPERACION_OMM.Models;

namespace OPERACION_OMM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoCambioController : ControllerBase
    {
        private readonly DBApiContext _context;

        public TipoCambioController(DBApiContext context)
        {
            _context = context;
        }

        // GET: api/TipoCambio
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipoCambio>>> GetTipoCambios()
        {
            return await _context.TipoCambios.ToListAsync();
        }

        // GET: api/TipoCambio/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TipoCambio>> GetTipoCambio(int id)
        {
            var tipoCambio = await _context.TipoCambios.FindAsync(id);

            if (tipoCambio == null)
            {
                return NotFound();
            }

            return tipoCambio;
        }

        // PUT: api/TipoCambio/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTipoCambio(int id, TipoCambio tipoCambio)
        {
            if (id != tipoCambio.Id)
            {
                return BadRequest();
            }

            _context.Entry(tipoCambio).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TipoCambioExists(id))
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

        // POST: api/TipoCambio
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TipoCambio>> PostTipoCambio(TipoCambio tipoCambio)
        {
            _context.TipoCambios.Add(tipoCambio);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTipoCambio", new { id = tipoCambio.Id }, tipoCambio);
        }

        // DELETE: api/TipoCambio/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipoCambio(int id)
        {
            var tipoCambio = await _context.TipoCambios.FindAsync(id);
            if (tipoCambio == null)
            {
                return NotFound();
            }

            _context.TipoCambios.Remove(tipoCambio);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TipoCambioExists(int id)
        {
            return _context.TipoCambios.Any(e => e.Id == id);
        }
    }
}
