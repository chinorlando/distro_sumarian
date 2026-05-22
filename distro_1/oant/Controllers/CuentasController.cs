using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OPERACION_OMM.Models;

namespace OPERACION_OMM.Controllers
{
    /// <summary>
    /// Gestión de Cuentas (Req. 1, 3, 4)
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CuentasController : ControllerBase
    {
        private readonly BdTransaccionesOmmContext _context;
        public CuentasController(BdTransaccionesOmmContext context) => _context = context;

        /// <summary>
        /// 4) CONSULTA DE SALDOS: Listado de todas las cuentas registradas.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cuenta>>> GetCuentas() => await _context.Cuenta.ToListAsync();

        /// <summary>
        /// 1) REGISTRAR CUENTA: Crea una cuenta bancaria con saldo inicial en 0.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Cuenta>> PostCuenta(Cuenta cuenta)
        {
            if (string.IsNullOrEmpty(cuenta.NroCuenta)) return BadRequest("El número de cuenta es obligatorio.");
            
            cuenta.Saldo = 0; // Por seguridad, el saldo inicial siempre es 0
            _context.Cuenta.Add(cuenta);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Cuenta registrada con éxito", cuenta });
        }

        /// <summary>
        /// 3) TRANSFERENCIA: Mueve saldo entre dos cuentas y registra el historial.
        /// </summary>
        [HttpPost("transferencia")]
        public async Task<IActionResult> Transferir([FromBody] Transferencia req)
        {
            if (req.Monto <= 0) return BadRequest("El monto debe ser mayor a cero.");
            if (req.CuentaOrigen == req.CuentaDestino) return BadRequest("Las cuentas de origen y destino deben ser diferentes.");

            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var oriy = await _context.Cuenta.FindAsync(req.CuentaOrigen);
                var dest = await _context.Cuenta.FindAsync(req.CuentaDestino);

                if (oriy == null || dest == null) return BadRequest("Una o ambas cuentas no existen.");
                if (oriy.Saldo < req.Monto) return BadRequest("Saldo insuficiente en la cuenta de origen.");

                // Ejecución de la transferencia
                oriy.Saldo -= req.Monto;
                dest.Saldo += req.Monto;

                var ahora = DateTime.Now;
                _context.Movimiento.AddRange(
                    new Movimiento { NroCuenta = req.CuentaOrigen, Fecha = ahora, Tipo = "D", Importe = req.Monto },
                    new Movimiento { NroCuenta = req.CuentaDestino, Fecha = ahora.AddMilliseconds(10), Tipo = "A", Importe = req.Monto }
                );

                await _context.SaveChangesAsync();
                await tx.CommitAsync();
                return Ok(new { message = "Transferencia realizada con éxito" });
            }
            catch (Exception ex) 
            { 
                await tx.RollbackAsync(); 
                return BadRequest($"Error en la transacción: {ex.Message}"); 
            }
        }
    }
}