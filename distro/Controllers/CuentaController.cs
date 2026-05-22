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
    public class CuentaController : ControllerBase
    {
        private readonly DBApiContext _context;

        public CuentaController(DBApiContext context)
        {
            _context = context;
        }

        // GET: api/Cuenta
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cuentum>>> GetCuenta()
        {
            return await _context.Cuenta.ToListAsync();
        }

        // GET: api/Cuenta/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cuentum>> GetCuentum(string id)
        {
            var cuentum = await _context.Cuenta.FindAsync(id);

            if (cuentum == null)
            {
                return NotFound();
            }

            return cuentum;
        }

        // PUT: api/Cuenta/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCuentum(string id, Cuentum cuentum)
        {
            if (id != cuentum.NroCuenta)
            {
                return BadRequest();
            }

            _context.Entry(cuentum).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CuentumExists(id))
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

        // POST: api/Cuenta
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Cuentum>> PostCuentum(Cuentum cuentum)
        {
            _context.Cuenta.Add(cuentum);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CuentumExists(cuentum.NroCuenta))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCuentum", new { id = cuentum.NroCuenta }, cuentum);
        }

        // DELETE: api/Cuenta/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCuentum(string id)
        {
            var cuentum = await _context.Cuenta.FindAsync(id);
            if (cuentum == null)
            {
                return NotFound();
            }

            _context.Cuenta.Remove(cuentum);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CuentumExists(string id)
        {
            return _context.Cuenta.Any(e => e.NroCuenta == id);
        }
    }
}
