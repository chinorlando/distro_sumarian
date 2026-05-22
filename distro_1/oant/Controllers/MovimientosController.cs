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
    /// Gestión de Movimientos (Req. 2, 5)
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientosController : ControllerBase
    {
        private readonly BdTransaccionesOmmContext _context;
        public MovimientosController(BdTransaccionesOmmContext context) => _context = context;

        /// <summary>
        /// 5) CONSULTA DE MOVIMIENTOS: Historial completo de una cuenta específica.
        /// </summary>
        [HttpGet("cuenta/{nroCuenta}")]
        public async Task<ActionResult<IEnumerable<Movimiento>>> GetHistorial(string nroCuenta)
        {
            return await _context.Movimiento
                .Where(m => m.NroCuenta == nroCuenta)
                .OrderByDescending(m => m.Fecha)
                .ToListAsync();
        }

        /// <summary>
        /// 2) OPERACIONES (DEPÓSITO/RETIRO): Registra movimiento y actualiza el saldo de la cuenta.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> PostOperacion(Movimiento mov)
        {
            if (mov.Importe <= 0) return BadRequest("El importe debe ser mayor a cero.");
            
            var cta = await _context.Cuenta.FindAsync(mov.NroCuenta);
            if (cta == null) return BadRequest("La cuenta no existe.");

            // Validar retiro (D)
            if (mov.Tipo == "D" && cta.Saldo < mov.Importe) 
                return BadRequest("Saldo insuficiente para realizar el retiro.");

            // Validar tipos permitidos
            if (mov.Tipo != "A" && mov.Tipo != "D")
                return BadRequest("Tipo de movimiento inválido. Use 'A' para Abono o 'D' para Débito.");

            // Actualizar saldo: suma si es Abono (A), resta si es Débito (D)
            cta.Saldo += (mov.Tipo == "A" ? 1 : -1) * mov.Importe;
            
            mov.Fecha = DateTime.Now;
            _context.Movimiento.Add(mov);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Operación exitosa", saldoActual = cta.Saldo });
        }
    }
}