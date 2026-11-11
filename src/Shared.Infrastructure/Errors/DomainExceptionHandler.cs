// [TP Microservicios E-Commerce v7] Agrupa infraestructura transversal reutilizable: errores, logs, persistencia y correlación.
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Shared.Contracts.Errors;
using Serilog;

namespace Shared.Infrastructure.Errors;

public class DomainExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not DomainException domainException)
        {
            return false;
        }

        var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault() ?? context.TraceIdentifier;
        var problem = new ApiProblemDetails(domainException, correlationId)
        {
            Instance = context.Request.Path
        };

        // Los errores previstos de reglas de negocio no son fallas de infraestructura: se registran como Warning.
        Log.Warning("Error de negocio {ErrorCode} en {Endpoint}", domainException.ErrorCode, context.Request.Path);
        context.Response.StatusCode = domainException.StatusCode;
        await context.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
