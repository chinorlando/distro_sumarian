# Guía de Implementación

Este documento detalla paso a paso todas las acciones realizadas para configurar y desarrollar el sistema de operaciones bancarias solicitado, utilizando **SQL Server**, **.NET 8 Web API** y **Entity Framework Core**.

---

## 1. Configuración de la Base de Datos (SQL Server)

Se creó la base de datos `BD` 

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
  "DefaultConnection": "Server=DESKTOP-5TBM2GV\\SQLSERVER2019;Database=BD_TRANSACCIONES_OMM;User Id=USUARIO;Password=PASSWORD;TrustServerCertificate=True;"
}
```

---

## 5. Ingeniería Inversa (Scaffolding de Modelos)

Se generaron las clases de C# automáticamente desde la base de datos existente usando la **Consola del Administrador de Paquetes**:

```powershell
Scaffold-DbContext "Name=DefaultConnection" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -Force -NoPluralize
```
*Nota: Se usó `-NoPluralize` para mantener los nombres de las tablas en español (`Cuenta` y `Movimiento`).*

---

## 6. Configuraciones Críticas en Program.cs

### A. Registro del Contexto de Base de Datos
```csharp
builder.Services.AddDbContext<BdTransaccionesOmmContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

### B. Habilitación de CORS (Cross-Origin Resource Sharing)
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
*Nota: También se habilitó `<GenerateDocumentationFile>true</GenerateDocumentationFile>` en el archivo `.csproj`.*


---

## 7. Implementación de los 5 Requerimientos (Código Final)

### 1) Registrar Cuenta (`POST /api/Cuentas`)
Crea una nueva cuenta con saldo inicial forzado a 0 y valida datos.
```csharp
[HttpPost]
public async Task<ActionResult<Cuenta>> PostCuenta(Cuenta cuenta)
{
    if (string.IsNullOrEmpty(cuenta.NroCuenta)) return BadRequest("El número de cuenta es obligatorio.");
    
    cuenta.Saldo = 0; // Por seguridad, el saldo inicial siempre es 0
    _context.Cuenta.Add(cuenta);
    await _context.SaveChangesAsync();
    return Ok(new { message = "Cuenta registrada con éxito", cuenta });
}
```

### 2) Depósitos y Retiros (`POST /api/Movimientos`)
Valida montos positivos, existencia de cuenta y saldo suficiente para retiros.
```csharp
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
```

CODIGO MUY OPTIMIZADO.
```csharp
[HttpPost]
public async Task<ActionResult> PostOperacion(Movimiento mov)
{
    if (mov.Importe <= 0) return BadRequest("El monto debe ser > 0");
    var cta = await _context.Cuenta.FindAsync(mov.NroCuenta);
    if (cta == null) return BadRequest("Cuenta no existe");
    if (mov.Tipo == "D" && cta.Saldo < mov.Importe) return BadRequest("Saldo insuficiente");

    cta.Saldo += (mov.Tipo == "A" ? 1 : -1) * mov.Importe;
    mov.Fecha = DateTime.Now;
    _context.Movimiento.Add(mov);
    await _context.SaveChangesAsync();
    return Ok(new { message = "Éxito", saldoActual = cta.Saldo });
}
```

### 3) Transferencias (`POST /api/Cuentas/transferencia`)
Mueve saldo entre dos cuentas usando transacciones para seguridad total.
```csharp
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
```

### 4) Consulta de Saldos (`GET /api/Cuentas`)
Listado de todas las cuentas registradas con su saldo actual.
```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<Cuenta>>> GetCuentas() => await _context.Cuenta.ToListAsync();
```

### 5) Consulta de Movimientos (`GET /api/Movimientos/cuenta/{nroCuenta}`)
Historial de una cuenta específica ordenado cronológicamente.
```csharp
[HttpGet("cuenta/{nroCuenta}")]
public async Task<ActionResult<IEnumerable<Movimiento>>> GetHistorial(string nroCuenta)
{
    return await _context.Movimiento
        .Where(m => m.NroCuenta == nroCuenta)
        .OrderByDescending(m => m.Fecha)
        .ToListAsync();
}
```

---

## 8. Pulido Final y Buenas Prácticas

1.  **Validaciones de Entrada**: Se añadieron protecciones contra montos negativos y nulos.
2.  **Transaccionalidad**: Se implementaron bloques `Transaction` para asegurar la integridad de los datos en transferencias.
3.  **Documentación Swagger**: Se configuró la API para mostrar descripciones claras de cada función en la interfaz web.
4.  **Optimización JSON**: Se habilitó `IgnoreCycles` y `[JsonIgnore]` para simplificar los objetos enviados y recibidos.
5.  **Clean Code**: Se refactorizaron los métodos para que sean más cortos y legibles.
