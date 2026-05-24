using Microsoft.EntityFrameworkCore;
using OPERACION_OMM.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. Añadir el servicio CORS con una política específica
builder.Services.AddCors(options =>
{
    options.AddPolicy("MiPoliticaCORS", policy =>
    {
        policy.AllowAnyOrigin() // Dominios permitidos
              .AllowAnyHeader()  // Permite cualquier cabecera
              .AllowAnyMethod(); // Permite cualquier método (GET, POST, PUT, etc.)
    });
});


// Add services to the container.

//builder.Services.AddControllers();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddDbContext<BdContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseCors("MiPoliticaCORS");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
