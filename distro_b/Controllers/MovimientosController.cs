using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using OPERACION_OMM.Models;

namespace OPERACION_OMM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientosController : ControllerBase
    {
        private readonly BdContext _context;

        public MovimientosController(BdContext context)
        {
            _context = context;
        }

        // GET: api/Movimientos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movimiento>>> GetMovimiento()
        {
            return await _context.Movimiento.ToListAsync();
        }

        // GET: api/Movimientos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Movimiento>> GetMovimiento(string id)
        {
            var movimiento = await _context.Movimiento.FindAsync(id);

            if (movimiento == null)
            {
                return NotFound();
            }

            return movimiento;
        }

        //// PUT: api/Movimientos/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutMovimiento(string id, Movimiento movimiento)
        //{
        //    if (id != movimiento.NroCuenta)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(movimiento).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!MovimientoExists(id))
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

        // POST: api/Movimientos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Movimiento>> PostMovimiento(Movimiento movimiento)
        {
            if (movimiento.Import <= 0)
                return Conflict(" el importe debe ser mayor a cero");

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var cta = await _context.Cuenta.FindAsync(movimiento.NroCuenta);
                if (cta == null) return Conflict("La cuenta no existe");

                // validar retiro 
                if (movimiento.Tipo == "D" && cta.Saldo < movimiento.Import)
                    return Conflict("Saldo insuficiente en la cuenta");

                // Actualizar saldo: suma si es Abono (A), resta si es Débito (D)
                cta.Saldo += (movimiento.Tipo =="A" ? 1 : -1) * movimiento.Import;

                movimiento.Fecha = DateTime.Now;
                _context.Movimiento.Add(movimiento);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new { message = "Operacion exitosa", nuevoSaldo = cta.Saldo});
               
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { error = "Error interno", detalle = ex.Message });
            }


            //_context.Movimiento.Add(movimiento);
            //try
            //{
            //    await _context.SaveChangesAsync();
            //}
            //catch (DbUpdateException)
            //{
            //    if (MovimientoExists(movimiento.NroCuenta))
            //    {
            //        return Conflict();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            //return CreatedAtAction("GetMovimiento", new { id = movimiento.NroCuenta }, movimiento);
        }

        //// DELETE: api/Movimientos/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteMovimiento(string id)
        //{
        //    var movimiento = await _context.Movimiento.FindAsync(id);
        //    if (movimiento == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Movimiento.Remove(movimiento);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}
        
        /// <summary>
        /// 5 listado de movimientos
        /// </summary>
        /// <param name="nroCuenta"></param>
        /// <returns>list</returns>
        [HttpGet("cuenta/{nroCuenta}")]
        public async Task<ActionResult<IEnumerable<Movimiento>>> HistorialCuenta(string nroCuenta)
        {
            var list = await _context.Movimiento
                .Where(t => t.NroCuenta == nroCuenta)
                .OrderByDescending(t => t.Fecha)
                .ToListAsync();
            if (list == null || !list.Any()) return NotFound("no se encontraron movimeintos para esta cuenta");
            
            return list;
        }

        private bool MovimientoExists(string id)
        {
            return _context.Movimiento.Any(e => e.NroCuenta == id);
        }
    }
}
