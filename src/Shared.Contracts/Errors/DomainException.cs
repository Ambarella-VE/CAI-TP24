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

    /// <summary>Selecciona la excepción concreta a partir de la categoría centralizada del código.</summary>
    public static DomainException FromCode(string errorCode) => ErrorCatalog.Find(errorCode).Category switch
    {
        ErrorCategory.NotFound => new NotFoundException(errorCode),
        ErrorCategory.Validation => new ValidationException(errorCode),
        ErrorCategory.BusinessRule => new BusinessRuleException(errorCode),
        _ => new DomainException(errorCode)
    };
}

/// <summary>Representa recursos inexistentes y se traduce a una respuesta HTTP 404.</summary>
public sealed class NotFoundException(string errorCode) : DomainException(errorCode);

/// <summary>Representa datos de entrada inválidos y se traduce a una respuesta HTTP 400.</summary>
public sealed class ValidationException(string errorCode) : DomainException(errorCode);

/// <summary>Representa reglas de negocio incumplidas, como duplicados o falta de stock.</summary>
public sealed class BusinessRuleException(string errorCode) : DomainException(errorCode);
