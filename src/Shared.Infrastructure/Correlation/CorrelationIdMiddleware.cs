// [TP Microservicios E-Commerce v7] Agrupa infraestructura transversal reutilizable: errores, logs, persistencia y correlación.
using Microsoft.AspNetCore.Http;
using Serilog.Context;
using Serilog;
using System.Diagnostics;

namespace Shared.Infrastructure.Correlation;

public class CorrelationIdMiddleware
{
    private const string HeaderName = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[HeaderName].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
            context.Request.Headers[HeaderName] = correlationId;
        }

        context.Response.Headers[HeaderName] = correlationId;

        // El contexto de Serilog permite que cada log de la solicitud incluya el mismo identificador.
        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            var stopwatch = Stopwatch.StartNew();
            // Se registra el inicio y el final para que la duración sea rastreable incluso fuera de la consola de desarrollo.
            Log.Information("Inicio de request en {Endpoint}", context.Request.Path);
            try
            {
                await _next(context);
            }
            finally
            {
                Log.Information("Fin de request en {Endpoint} con status {StatusCode} en {DurationMs} ms", context.Request.Path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
