// [TP Microservicios E-Commerce v7] Convierte fallas no previstas en un error 500 seguro sin exponer detalles internos.
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Serilog;
using Shared.Contracts.Errors;

namespace Shared.Infrastructure.Errors;

public class UnexpectedExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        // Cada API conserva su código interno de la consigna aun cuando el error venga de infraestructura.
        var errorCode = context.Request.Path.Value?.Split('/', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() switch
        {
            "products" => "PRD-005",
            "users" => "USR-006",
            "orders" => "ORD-007",
            "cart" => "CRT-005",
            "notifications" => "NTF-004",
            _ => "ORD-007"
        };
        var correlationId = context.Request.Headers["X-Correlation-Id"].FirstOrDefault() ?? context.TraceIdentifier;
        var problem = new ApiProblemDetails(DomainException.FromCode(errorCode), correlationId) { Instance = context.Request.Path };

        // El detalle técnico queda en el log; la respuesta pública usa el mensaje seguro del catálogo.
        Log.Error(exception, "Error inesperado {ErrorCode} en {Endpoint}", errorCode, context.Request.Path);
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
