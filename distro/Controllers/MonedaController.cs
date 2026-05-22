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
    public class MonedaController : ControllerBase
    {
        private readonly DBApiContext _context;

        public MonedaController(DBApiContext context)
        {
            _context = context;
        }

        // GET: api/Moneda
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Monedum>>> GetMoneda()
        {
            return await _context.Moneda.ToListAsync();
        }

        // GET: api/Moneda/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Monedum>> GetMonedum(string id)
        {
            var monedum = await _context.Moneda.FindAsync(id);

            if (monedum == null)
            {
                return NotFound();
            }

            return monedum;
        }

        // PUT: api/Moneda/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMonedum(string id, Monedum monedum)
        {
            if (id != monedum.Codigo)
            {
                return BadRequest();
            }

            _context.Entry(monedum).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MonedumExists(id))
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

        // POST: api/Moneda
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Monedum>> PostMonedum(Monedum monedum)
        {
            _context.Moneda.Add(monedum);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (MonedumExists(monedum.Codigo))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetMonedum", new { id = monedum.Codigo }, monedum);
        }

        // DELETE: api/Moneda/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMonedum(string id)
        {
            var monedum = await _context.Moneda.FindAsync(id);
            if (monedum == null)
            {
                return NotFound();
            }

            _context.Moneda.Remove(monedum);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MonedumExists(string id)
        {
            return _context.Moneda.Any(e => e.Codigo == id);
        }
    }
}
