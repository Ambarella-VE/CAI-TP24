using Serilog;
// [TP Microservicios E-Commerce v7] Compone Users.API: registra dependencias, pipeline HTTP, Swagger, errores, logs y health checks.
using Shared.Infrastructure.Correlation;

using Shared.Infrastructure.Errors;
using Shared.Infrastructure.Health;
using Shared.Infrastructure.Logging;
using Users.API.Services;

var builder = WebApplication.CreateBuilder(args);
builder.AddSharedSerilog();

builder.Services.AddControllers();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSwaggerGen(options => { var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml"; options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile)); });
builder.Services.AddExceptionHandler<DomainExceptionHandler>();
builder.Services.AddExceptionHandler<UnexpectedExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseExceptionHandler();

app.UseAuthorization();

app.MapControllers();
app.MapSharedHealthChecks();

app.Run();
