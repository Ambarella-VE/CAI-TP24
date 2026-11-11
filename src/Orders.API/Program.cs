using Serilog;
// [TP Microservicios E-Commerce v7] Compone Orders.API: registra dependencias, pipeline HTTP, Swagger, errores, logs y health checks.
using Orders.API.Clients;
using Orders.API.Services;
using Shared.Contracts.Configuration;
using Shared.Infrastructure.Correlation;

using Shared.Infrastructure.Errors;
using Shared.Infrastructure.Health;
using Shared.Infrastructure.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.AddSharedSerilog();

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<CorrelationIdDelegatingHandler>();
var serviceOptions = builder.Configuration.GetSection(ServiceOptions.SectionName).Get<ServiceOptions>() ?? new ServiceOptions();
builder.Services.AddHttpClient<IUsersClient, UsersHttpClient>(client => client.BaseAddress = new Uri(serviceOptions.Users)).AddHttpMessageHandler<CorrelationIdDelegatingHandler>();
builder.Services.AddHttpClient<IProductsClient, ProductsHttpClient>(client => client.BaseAddress = new Uri(serviceOptions.Products)).AddHttpMessageHandler<CorrelationIdDelegatingHandler>();
builder.Services.AddSingleton<OrderService>();
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
