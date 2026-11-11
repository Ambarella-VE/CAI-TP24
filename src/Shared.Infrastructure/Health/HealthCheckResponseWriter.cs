// [TP Microservicios E-Commerce v7] Escribe la respuesta JSON de los health checks para informar el estado operativo requerido por la consigna.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Shared.Infrastructure.Health;

public static class HealthCheckResponseWriter
{
    public static Task Write(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsJsonAsync(new
        {
            status = report.Status.ToString(),
            entries = report.Entries.ToDictionary(entry => entry.Key, entry => entry.Value.Status.ToString())
        });
    }
}

public static class HealthCheckExtensions
{
    public static void MapSharedHealthChecks(this IEndpointRouteBuilder app)
    {
        var options = new HealthCheckOptions { ResponseWriter = HealthCheckResponseWriter.Write };
        app.MapHealthChecks("/health", options);
        app.MapHealthChecks("/health/live", options);
        app.MapHealthChecks("/health/ready", options);
    }
}
