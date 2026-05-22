using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OPERACION_OMM_NEW.Models;

namespace OPERACION_OMM_NEW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuentasController : ControllerBase
    {
        private readonly DBApiContext _context;

        public CuentasController(DBApiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 4) CONSULTA DE SALDOS (Listado general de cuentas)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cuenta>>> GetCuenta()
        {
            return await _context.Cuenta.ToListAsync();
        }

        /// <summary>
        /// Obtiene el detalle de una cuenta específica por su ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Cuenta>> GetCuenta(string id)
        {
            var cuenta = await _context.Cuenta.FindAsync(id);
            if (cuenta == null) return NotFound();
            return cuenta;
        }

        /// <summary>
        /// 1) REGISTRAR CUENTA: Saldo inicial siempre en 0.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Cuenta>> PostCuenta(Cuenta cuenta)
        {
            if (await _context.Cuenta.AnyAsync(c => c.NroCuenta == cuenta.NroCuenta))
                return Conflict("El número de cuenta ya existe.");

            cuenta.Saldo = 0; 
            _context.Cuenta.Add(cuenta);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCuenta", new { id = cuenta.NroCuenta }, cuenta);
        }

        /// <summary>
        /// 3) REALIZAR TRANSFERENCIA: Mueve saldo entre dos cuentas con conversión automática.
        /// </summary>
        [HttpPost("transferencia")]
        public async Task<IActionResult> Transferir([FromBody] TransferenciaRequest req)
        {
            if (req.Monto <= 0) return BadRequest("El monto debe ser mayor a cero.");

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var ori = await _context.Cuenta.FindAsync(req.CuentaOrigen);
                var des = await _context.Cuenta.FindAsync(req.CuentaDestino);

                if (ori == null || des == null) return BadRequest("Una o ambas cuentas no existen.");
                if ((ori.Saldo ?? 0) < req.Monto) return BadRequest("Saldo insuficiente en la cuenta de origen.");

                decimal montoFinal = req.Monto;
                decimal tasa = 1.0m;

                // LÓGICA DE CONVERSIÓN: Si las monedas son distintas, buscamos el tipo de cambio
                if (ori.Moneda != des.Moneda)
                {
                    var tc = await _context.TipoCambio
                        .Where(t => t.MonedaOrigen == ori.Moneda && t.MonedaDestino == des.Moneda)
                        .OrderByDescending(t => t.Fecha) 
                        .FirstOrDefaultAsync();

                    if (tc == null) return BadRequest($"No existe tipo de cambio de {ori.Moneda} a {des.Moneda}");
                    
                    tasa = tc.Tasa;
                    montoFinal = req.Monto * tasa;
                }

                // Actualizar saldos
                ori.Saldo = (ori.Saldo ?? 0) - req.Monto;
                des.Saldo = (des.Saldo ?? 0) + montoFinal;

                // Registrar movimientos (Débito y Abono)
                var ahora = DateTime.Now;
                _context.Movimiento.AddRange(
                    new Movimiento { NroCuenta = req.CuentaOrigen, Fecha = ahora, Tipo = "D", Importe = req.Monto },
                    new Movimiento { NroCuenta = req.CuentaDestino, Fecha = ahora.AddMilliseconds(50), Tipo = "A", Importe = montoFinal }
                );

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return Ok(new { message = "Transferencia exitosa", tasaAplicada = tasa, recibidoEnDestino = montoFinal });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        private bool CuentaExists(string id)
        {
            return _context.Cuenta.Any(e => e.NroCuenta == id);
        }
    }
}