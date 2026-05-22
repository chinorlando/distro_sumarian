using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OPERACION_OMM_NEW.Models;

namespace OPERACION_OMM_NEW.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipoCambiosController : ControllerBase
    {
        private readonly DBApiContext _context;
        public TipoCambiosController(DBApiContext context) => _context = context;

        /// <summary>
        /// Obtiene el listado de todos los tipos de cambio registrados.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipoCambio>>> GetTipoCambios() => await _context.TipoCambio.ToListAsync();

        /// <summary>
        /// Registra una nueva tasa de cambio entre dos monedas.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TipoCambio>> PostTipoCambio(TipoCambio tipoCambio)
        {
            _context.TipoCambio.Add(tipoCambio);
            await _context.SaveChangesAsync();
            return Ok(tipoCambio);
        }
    }
}