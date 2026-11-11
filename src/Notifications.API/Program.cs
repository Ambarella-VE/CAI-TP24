// [TP Microservicios E-Commerce v7] Compone Notifications.API: registra dependencias, pipeline HTTP, Swagger, errores, logs y health checks.
using Notifications.API.Services;
using Notifications.API.Clients;
using Shared.Contracts.Configuration;
using Shared.Infrastructure.Correlation;

using Shared.Infrastructure.Errors;
using Shared.Infrastructure.Health;
using Shared.Infrastructure.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.AddSharedSerilog();

builder.Services.AddControllers();
var serviceOptions = builder.Configuration.GetSection(ServiceOptions.SectionName).Get<ServiceOptions>() ?? new ServiceOptions();
builder.Services.AddHttpClient<IUsersClient, UsersHttpClient>(client => client.BaseAddress = new Uri(serviceOptions.Users));
builder.Services.AddSingleton<NotificationService>();
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<DomainExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();

var app = builder.Build();

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
