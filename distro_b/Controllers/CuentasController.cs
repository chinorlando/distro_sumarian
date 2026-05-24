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
    public class CuentasController : ControllerBase
    {
        private readonly BdContext _context;

        public CuentasController(BdContext context)
        {
            _context = context;
        }

        // GET: api/Cuentas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cuenta>>> GetCuenta()
        {
            return await _context.Cuenta.ToListAsync();
        }

        // GET: api/Cuentas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cuenta>> GetCuenta(string id)
        {
            var cuenta = await _context.Cuenta.FindAsync(id);

            if (cuenta == null)
            {
                return NotFound();
            }

            return cuenta;
        }

        //// PUT: api/Cuentas/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutCuenta(string id, Cuenta cuenta)
        //{
        //    if (id != cuenta.NroCuenta)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(cuenta).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!CuentaExists(id))
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

        // POST: api/Cuentas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Cuenta>> PostCuenta(Cuenta cuenta)
        {
            if (await _context.Cuenta.AnyAsync(c => c.NroCuenta == cuenta.NroCuenta))
                return Conflict("el numero de cuenta ya existe");
            if (await _context.Cuenta.AnyAsync(c => c.Saldo < 0))
                return Conflict("esta creando el saldo con un saldo negativo");

            _context.Cuenta.Add(cuenta);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CuentaExists(cuenta.NroCuenta))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCuenta", new { id = cuenta.NroCuenta }, cuenta);
        }

        //// DELETE: api/Cuentas/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteCuenta(string id)
        //{
        //    var cuenta = await _context.Cuenta.FindAsync(id);
        //    if (cuenta == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Cuenta.Remove(cuenta);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}


        [HttpPost("transferencia")]
        public async Task<IActionResult> Transferir([FromBody] Transferencia tra)
        {
            if (tra.Monto <= 0) return BadRequest("el monto debe ser mayor a cero");
            if (tra.CuentaOrigen == tra.CuentaDestino) return BadRequest("las cuanestas deben ser diferentes");

            var ori = await _context.Cuenta.FindAsync(tra.CuentaOrigen);
            var des = await _context.Cuenta.FindAsync(tra.CuentaDestino);

            if (ori == null || des == null) return BadRequest("Cuentas no existen.");
            if ((ori.Saldo) < tra.Monto) return BadRequest("Saldo insuficiente.");

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                decimal montoFinal = tra.Monto;
                decimal tasa = 1.0m;

                // LÓGICA DE CONVERSIÓN: Si las monedas son distintas, buscamos el tipo de cambio
                if (ori.Moneda != des.Moneda)
                {
                    // buscamos las conversiones y tasas en tipo de cambio
                    var tc = await _context.TipoCambio
                        .Where(t => t.MonedaOrigen == ori.Moneda && t.MonedaDestino == des.Moneda)
                        .OrderByDescending(t => t.Fecha)
                        .FirstOrDefaultAsync();
                    if (tc == null) return BadRequest($"No existe tipo de cambio de {ori.Moneda} a {des.Moneda}");
                    // si existe entonces hacemos la conversion
                    tasa = tc.Tasa;
                    montoFinal = tra.Monto * tasa;
                }

                // actualizamos saldos en las cuentas
                ori.Saldo = ori.Saldo - tra.Monto;
                des.Saldo = des.Saldo + montoFinal;

                // registramos los movimientos
                var ahora = DateTime.Now;
                _context.Movimiento.AddRange(
                    new Movimiento { NroCuenta = tra.CuentaOrigen, Fecha = ahora, Tipo = "D", Import = tra.Monto },
                    new Movimiento { NroCuenta = tra.CuentaDestino, Fecha = ahora.AddMilliseconds(10), Tipo = "A", Import = montoFinal }
                );
                await _context.SaveChangesAsync();

                await tx.CommitAsync();
                return Ok(new { mensage = "realizacon con exito" });
            }
            catch (Exception e)
            {
                await tx.RollbackAsync();
                return StatusCode(500, $"Error: {e.Message}");
            }
        }

        private bool CuentaExists(string id)
        {
            return _context.Cuenta.Any(e => e.NroCuenta == id);
        }
    }
}
