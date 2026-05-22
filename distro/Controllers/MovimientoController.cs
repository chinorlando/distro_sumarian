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
    public class MovimientoController : ControllerBase
    {
        private readonly DBApiContext _context;
        private readonly ILogger<MovimientoController> _logger;

        public MovimientoController(ILogger<MovimientoController> logger, DBApiContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: api/Movimiento
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movimiento>>> GetMovimientos()
        {
            return await _context.Movimientos.ToListAsync();
        }

        // GET: api/Movimiento/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Movimiento>> GetMovimiento(DateTime id)
        {
            var movimiento = await _context.Movimientos.FindAsync(id);

            if (movimiento == null)
            {
                return NotFound();
            }

            return movimiento;
        }

        // PUT: api/Movimiento/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovimiento(DateTime id, Movimiento movimiento)
        {
            if (id != movimiento.Fecha)
            {
                return BadRequest();
            }

            _context.Entry(movimiento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovimientoExists(id))
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

        // POST: api/Movimiento
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Movimiento>> PostMovimiento(MovimientoRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (request.Debito != true)
                {
                    request.Movimiento.Tipo = "A";
                }
                else
                {
                    request.Movimiento.Tipo = "D";
                    request.Movimiento.Importe = -Math.Abs((decimal)request.Movimiento.Importe);
                }

                request.Movimiento.Fecha = DateTime.Now;

                //request.Movimiento.Tipo = request.Movimiento.Importe > 0 ? "A" : "D";
                _context.Movimientos.Add(request.Movimiento);

                await _context.SaveChangesAsync();

                await _context.Database.ExecuteSqlRawAsync("EXEC SumarizarMovimientosYActualizarCuentas");

                await transaction.CommitAsync();

                return CreatedAtAction("GetMovimiento", new { id = request.Movimiento.Fecha }, request.Movimiento);
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();

                if (MovimientoExists(request.Movimiento.Fecha))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        //public async Task<ActionResult<Movimiento>> PostMovimiento(Movimiento movimiento)
        //{
        //    _context.Movimientos.Add(movimiento);
        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException)
        //    {
        //        if (MovimientoExists(movimiento.Fecha))
        //        {
        //            return Conflict();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return CreatedAtAction("GetMovimiento", new { id = movimiento.Fecha }, movimiento);
        //}


        // POST: api/Movimiento/transferencia
        [HttpPost]
        [Route("transferencia")]
        public async Task<ActionResult<Movimiento>> PostMovimiento(MovimientoCuentas request)
        {
            _logger.LogInformation("Solicitud de transferencia recibida: {@Request}", request.CuentaOrigenID);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var result = await _context.Database.ExecuteSqlRawAsync(
                    "EXEC RealizarTransferencia @p0, @p1, @p2",
                    request.CuentaOrigenID, request.CuentaDestinoID, request.Monto);

                await transaction.CommitAsync();

                var movimiento = await _context.Movimientos
                    .OrderByDescending(m => m.Fecha)
                    .FirstOrDefaultAsync(m => m.NroCuenta == request.CuentaOrigenID);

                if (movimiento == null)
                {
                    return NotFound();
                }

                return CreatedAtAction("GetMovimiento", new { id = movimiento.Fecha }, movimiento);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error al actualizar la base de datos durante la transferencia.");
                await transaction.RollbackAsync();
                return Conflict();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado durante la transferencia.");
                await transaction.RollbackAsync();
                throw;
            }
        }

        // DELETE: api/Movimiento/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovimiento(DateTime id)
        {
            var movimiento = await _context.Movimientos.FindAsync(id);
            if (movimiento == null)
            {
                return NotFound();
            }

            _context.Movimientos.Remove(movimiento);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //obtiene el detalle, los mvimientos de una cuenta
        [HttpGet("lista/{nroCuenta}")]
        public async Task<ActionResult<IEnumerable<object>>> GetMovimientosPorNroCuenta(string nroCuenta)
        {
            var movimientos = await _context.Movimientos
                .Where(m => m.NroCuenta == nroCuenta)
                .Include(m => m.oCuenta)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();

            if (movimientos == null || movimientos.Count == 0)
            {
                return NotFound();
            }

            var result = movimientos.Select(m => new
            {
                m.Fecha,
                m.Tipo,
                m.Importe,
                m.NroCuenta,
                oCuenta = new
                {
                    m.oCuenta.NroCuenta,
                    m.oCuenta.Tipo,
                    m.oCuenta.Moneda,
                    m.oCuenta.Nombre,
                    m.oCuenta.Saldo
                }
            });

            return Ok(result);
        }

        private bool MovimientoExists(DateTime id)
        {
            return _context.Movimientos.Any(e => e.Fecha == id);
        }
    }
}
