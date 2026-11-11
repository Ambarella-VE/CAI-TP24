// [TP Microservicios E-Commerce v7] Compone Cart.API: registra dependencias, pipeline HTTP, Swagger, errores, logs y health checks.
using Cart.API.Clients;
using Cart.API.Services;
using Shared.Contracts.Configuration;
using Shared.Infrastructure.Correlation;
using Shared.Infrastructure.Errors;
using Shared.Infrastructure.Logging;
var builder = WebApplication.CreateBuilder(args);
builder.AddSharedSerilog();

builder.Services.AddControllers();
var serviceOptions = builder.Configuration.GetSection(ServiceOptions.SectionName).Get<ServiceOptions>() ?? new ServiceOptions();
builder.Services.AddHttpClient<IProductsClient, ProductsHttpClient>(client => client.BaseAddress = new Uri(serviceOptions.Products));
builder.Services.AddSingleton<CartService>();
builder.Services.AddSwaggerGen(); builder.Services.AddExceptionHandler<DomainExceptionHandler>(); builder.Services.AddProblemDetails(); builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<CorrelationIdMiddleware>(); app.UseExceptionHandler();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health"); app.MapHealthChecks("/health/live"); app.MapHealthChecks("/health/ready");

app.Run();
