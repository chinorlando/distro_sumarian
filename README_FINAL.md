# Guía de Implementación

Este documento detalla paso a paso todas las acciones realizadas para configurar y desarrollar el sistema de operaciones bancarias solicitado, utilizando **SQL Server**, **.NET 8 Web API** y **Entity Framework Core**.

---

## 1. Configuración de la Base de Datos (SQL Server)

Se creó la base de datos `BD`

---

## 2. Creación del Proyecto .NET

---

## 3. Instalación de Paquetes NuGet

Para habilitar la conexión a base de datos y las herramientas de generación de código, se instalaron los siguientes paquetes:

```bash
# Para el acceso a datos y Entity Framework
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.x
dotnet add package Microsoft.Data.SqlClient

# Para herramientas de diseño y scaffolding (generación de código)
dotnet add package Microsoft.EntityFrameworkCore.Design --version 8.0.x
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.x
```

---

## 4. Configuración de la Conexión (appsettings.json)

Se configuró la cadena de conexión apuntando a la instancia local de SQL Server:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=eso\\SQLSERVER22;Database=BD_;User Id=user;Password=pass;TrustServerCertificate=True;"
}
```

---

## 5. Ingeniería Inversa (Scaffolding de Modelos)

Se generaron las clases de C# automáticamente desde la base de datos con las 4 tablas usando la consola de NuGet:

```powershell
Scaffold-DbContext "Name=DefaultConnection" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Force -NoPluralize
```

---

## 6. Configuraciones Críticas en Program.cs

### A. Registro del Contexto de Base de Datos
```csharp
builder.Services.AddDbContext<DBApiContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

### B. Habilitación de CORS
```csharp
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll", policy => {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});
app.UseCors("AllowAll");
```

### C. Manejo de Ciclos de Referencia JSON
```csharp
builder.Services.AddControllers().AddJsonOptions(x => 
    x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
```

### D. Documentación XML en Swagger
```csharp
builder.Services.AddSwaggerGen(c => {
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});
```

---

## 7. Implementación de los 5 Requerimientos (Funciones Completas)

### 1) Registrar Cuenta (`POST /api/Cuentas`)
```csharp
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
```

### 2) Depósitos y Retiros (`POST /api/Movimientos`)
```csharp
[HttpPost]
public async Task<ActionResult> PostMovimiento(Movimiento mov)
{
    if (mov.Importe <= 0) return BadRequest("El importe debe ser mayor a cero.");
    if (mov.Tipo != "A" && mov.Tipo != "D") return BadRequest("Tipo inválido. Use 'A' o 'D'.");

    using var transaction = await _context.Database.BeginTransactionAsync();

    try
    {
        var cta = await _context.Cuenta.FindAsync(mov.NroCuenta);
        if (cta == null) return BadRequest("La cuenta no existe.");

        decimal saldoActual = cta.Saldo ?? 0;

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

        mov.Fecha = DateTime.Now;
        _context.Movimiento.Add(mov);

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return Ok(new { message = "Operación exitosa", nuevoSaldo = cta.Saldo });
    }
    catch (Exception ex)
    {
        await transaction.RollbackAsync();
        return StatusCode(500, new { error = "Error interno", detalle = ex.Message });
    }
}
```

### 3) Transferencias con Conversión (`POST /api/Cuentas/transferencia`)
```csharp
[HttpPost("transferencia")]
public async Task<IActionResult> Transferir([FromBody] TransferenciaRequest req)
{
    if (req.Monto <= 0) return BadRequest("El monto debe ser mayor a cero.");

    using var tx = await _context.Database.BeginTransactionAsync();
    try
    {
        var ori = await _context.Cuenta.FindAsync(req.CuentaOrigen);
        var des = await _context.Cuenta.FindAsync(req.CuentaDestino);

        if (ori == null || des == null) return BadRequest("Cuentas no existen.");
        if ((ori.Saldo ?? 0) < req.Monto) return BadRequest("Saldo insuficiente.");

        decimal montoFinal = req.Monto;
        decimal tasa = 1.0m;

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

        ori.Saldo = (ori.Saldo ?? 0) - req.Monto;
        des.Saldo = (des.Saldo ?? 0) + montoFinal;

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
```

### 4) Consulta de Saldos (`GET /api/Cuentas`)
```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<Cuenta>>> GetCuenta()
{
    return await _context.Cuenta.ToListAsync();
}
```

### 5) Consulta de Movimientos (`GET /api/Movimientos/cuenta/{nroCuenta}`)
```csharp
[HttpGet("cuenta/{nroCuenta}")]
public async Task<ActionResult<IEnumerable<Movimiento>>> GetHistorial(string nroCuenta)
{
    var movimientos = await _context.Movimiento
        .Where(m => m.NroCuenta == nroCuenta)
        .OrderByDescending(m => m.Fecha)
        .ToListAsync();

    if (movimientos == null || !movimientos.Any()) 
        return NotFound("No se encontraron movimientos para esta cuenta.");

    return movimientos;
}
```

---

## 8. Pulido Final y Buenas Prácticas

1.  **Validaciones de Entrada**: Se añadieron protecciones contra montos negativos y tipos de cuenta inválidos.
2.  **Transaccionalidad**: Se implementaron bloques `BeginTransactionAsync` para asegurar la integridad multimoneda.
3.  **Documentación Swagger**: Cada endpoint incluye un `/// <summary>` descriptivo en español.
4.  **Optimización JSON**: Se habilitó `IgnoreCycles` y `[JsonIgnore]` para facilitar la integración con el frontend.
5.  **Clean Code**: La lógica de negocio se centralizó en los controladores evitando duplicidad de código.

## 9. metodo de transferencia
```csharp
public class TransferenciaRequest
{
    public string CuentaOrigen { get; set; }
    public string CuentaDestino { get; set; }
    public decimal Monto { get; set; }
}
```