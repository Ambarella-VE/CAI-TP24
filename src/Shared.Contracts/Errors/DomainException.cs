// [TP Microservicios E-Commerce v7] Centraliza contratos técnicos reutilizables para responder de forma consistente a la consigna.
namespace Shared.Contracts.Errors;

public class DomainException : Exception
{
    public string ErrorCode { get; }
    public int StatusCode { get; }
    public ErrorCategory Category { get; }

    public DomainException(string errorCode)
        : this(ErrorCatalog.Find(errorCode))
    {
    }

    public DomainException(ErrorDefinition definition)
        : base(definition.Message)
    {
        ErrorCode = definition.Code;
        StatusCode = definition.StatusCode;
        Category = definition.Category;
    }
}
