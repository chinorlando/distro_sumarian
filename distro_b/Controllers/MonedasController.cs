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
    public class MonedasController : ControllerBase
    {
        private readonly BdContext _context;

        public MonedasController(BdContext context)
        {
            _context = context;
        }

        // GET: api/Monedas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Moneda>>> GetMoneda()
        {
            return await _context.Moneda.ToListAsync();
        }

        // GET: api/Monedas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Moneda>> GetMoneda(string id)
        {
            var moneda = await _context.Moneda.FindAsync(id);

            if (moneda == null)
            {
                return NotFound();
            }

            return moneda;
        }

        //// PUT: api/Monedas/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutMoneda(string id, Moneda moneda)
        //{
        //    if (id != moneda.Codigo)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(moneda).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!MonedaExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/Monedas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Moneda>> PostMoneda(Moneda moneda)
        {
            _context.Moneda.Add(moneda);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MonedaExists(moneda.Codigo))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetMoneda", new { id = moneda.Codigo }, moneda);
        }

        //// DELETE: api/Monedas/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteMoneda(string id)
        //{
        //    var moneda = await _context.Moneda.FindAsync(id);
        //    if (moneda == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Moneda.Remove(moneda);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        private bool MonedaExists(string id)
        {
            return _context.Moneda.Any(e => e.Codigo == id);
        }
    }
}
