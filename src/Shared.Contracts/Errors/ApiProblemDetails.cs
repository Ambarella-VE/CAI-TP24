// [TP Microservicios E-Commerce v7] Centraliza contratos técnicos reutilizables para responder de forma consistente a la consigna.
using Microsoft.AspNetCore.Mvc;

namespace Shared.Contracts.Errors;

public class ApiProblemDetails : ProblemDetails
{
    public string ErrorCode { get; init; }
    public string ErrorMessage { get; init; }
    public string CorrelationId { get; init; }

    public ApiProblemDetails(DomainException exception, string correlationId)
    {
        Status = exception.StatusCode;
        Title = exception.Category.ToString();
        Detail = exception.Message;
        ErrorCode = exception.ErrorCode;
        ErrorMessage = exception.Message;
        CorrelationId = correlationId;
    }
}
