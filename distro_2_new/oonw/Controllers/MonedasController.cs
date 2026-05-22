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
    public class MonedasController : ControllerBase
    {
        private readonly DBApiContext _context;
        public MonedasController(DBApiContext context) => _context = context;

        /// <summary>
        /// Obtiene el listado de todas las monedas registradas (BOB, USD, etc).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Moneda>>> GetMonedas() => await _context.Moneda.ToListAsync();

        /// <summary>
        /// Registra una nueva moneda en el sistema.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Moneda>> PostMoneda(Moneda moneda)
        {
            _context.Moneda.Add(moneda);
            await _context.SaveChangesAsync();
            return Ok(moneda);
        }
    }
}