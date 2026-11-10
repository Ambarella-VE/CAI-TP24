var builder = WebApplication.CreateBuilder(args);

// TODO: [TP v7 §5.1] Registrar Controllers, Swagger y ejemplos de Products.
// TODO: [TP v7 §5.2] Registrar el handler central de errores PRD-001 a PRD-005.
// TODO: [TP v7 §5.3-§5.5] Configurar Serilog, health checks y Correlation ID.

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
