// [TP Microservicios E-Commerce v7] Propaga el Correlation ID hacia las llamadas HTTP entre microservicios.
using Microsoft.AspNetCore.Http;

namespace Shared.Infrastructure.Correlation;

public class CorrelationIdDelegatingHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var correlationId = httpContextAccessor.HttpContext?.Request.Headers["X-Correlation-Id"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(correlationId))
        {
            // Reutilizar el mismo identificador permite seguir una operación desde Cart/Orders hasta Products/Users.
            request.Headers.TryAddWithoutValidation("X-Correlation-Id", correlationId);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
