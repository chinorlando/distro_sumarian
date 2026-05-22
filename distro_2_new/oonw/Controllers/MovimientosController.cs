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
    public class MovimientosController : ControllerBase
    {
        private readonly DBApiContext _context;

        public MovimientosController(DBApiContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene el listado de todos los movimientos registrados en el sistema.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movimiento>>> GetMovimiento()
        {
            return await _context.Movimiento.ToListAsync();
        }

        /// <summary>
        /// 5) CONSULTA DE MOVIMIENTOS POR CUENTA: Historial de una cuenta específica.
        /// </summary>
        [HttpGet("cuenta/{nroCuenta}")]
        public async Task<ActionResult<IEnumerable<Movimiento>>> GetHistorial(string nroCuenta)
        {
            var movimientos = await _context.Movimiento
                .Where(m => m.NroCuenta == nroCuenta)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();

            if (movimientos == null || !movimientos.Any()) return NotFound("No se encontraron movimientos para esta cuenta.");

            return movimientos;
        }

        /// <summary>
        /// 2) OPERACIONES DE DEPÓSITOS Y RETIROS: Versión Profesional con Transacciones
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> PostMovimiento(Movimiento mov)
        {
            // 1. Validaciones básicas de entrada
            if (mov.Importe <= 0) return BadRequest("El importe debe ser mayor a cero.");
            if (mov.Tipo != "A" && mov.Tipo != "D") return BadRequest("Tipo inválido. Use 'A' para Abono o 'D' para Débito.");

            // 2. Uso de Transacción para asegurar integridad total (Atomicidad)
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 3. Buscar la cuenta
                var cta = await _context.Cuenta.FindAsync(mov.NroCuenta);
                if (cta == null) return BadRequest("La cuenta no existe.");

                // Manejo de nulos en saldo
                decimal saldoActual = cta.Saldo ?? 0;

                // 4. Lógica de negocio mediante Switch (Más legible y escalable)
                switch (mov.Tipo)
                {
                    case "A": // Abono / Depósito
                        cta.Saldo = saldoActual + mov.Importe;
                        break;
                    case "D": // Débito / Retiro
                        if (saldoActual < mov.Importe) 
                            return BadRequest("Saldo insuficiente para realizar la operación.");
                        cta.Saldo = saldoActual - mov.Importe;
                        break;
                }

                // 5. Registrar el movimiento con la fecha actual
                mov.Fecha = DateTime.Now;
                _context.Movimiento.Add(mov);

                // 6. Guardar cambios en ambas tablas y confirmar transacción
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { 
                    message = "Operación realizada con éxito", 
                    nuevoSaldo = cta.Saldo 
                });
            }
            catch (Exception ex)
            {
                // Si algo falla, se deshacen todos los cambios automáticamente
                await transaction.RollbackAsync();
                return StatusCode(500, new { error = "Error interno del servidor", detalle = ex.Message });
            }
        }
    }
}