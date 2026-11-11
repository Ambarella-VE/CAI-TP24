using Serilog;
// [TP Microservicios E-Commerce v7] Compone Notifications.API: registra dependencias, pipeline HTTP, Swagger, errores, logs y health checks.
using Notifications.API.Services;
using Notifications.API.Clients;
using Notifications.API.Models;
using Shared.Contracts.Configuration;
using Shared.Infrastructure.Correlation;

using Shared.Infrastructure.Errors;
using Shared.Infrastructure.Health;
using Shared.Infrastructure.Logging;
using Shared.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
builder.AddSharedSerilog();

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<CorrelationIdDelegatingHandler>();
var serviceOptions = builder.Configuration.GetSection(ServiceOptions.SectionName).Get<ServiceOptions>() ?? new ServiceOptions();
builder.Services.AddHttpClient<IUsersClient, UsersHttpClient>(client => client.BaseAddress = new Uri(serviceOptions.Users)).AddHttpMessageHandler<CorrelationIdDelegatingHandler>();
builder.Services.AddSingleton<IEntityStore<Notification>, SqliteEntityStore<Notification>>();
builder.Services.AddSingleton<NotificationService>();
builder.Services.AddSwaggerGen(options => { var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml"; options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile)); options.OperationFilter<Shared.Infrastructure.Documentation.SwaggerExamplesOperationFilter>(); });
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
