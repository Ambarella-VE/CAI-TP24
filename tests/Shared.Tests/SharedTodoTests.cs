using Shared.Contracts.Errors;

namespace Shared.Tests;

public class ErrorCatalogTests
{
    [Fact]
    public void Find_debe_devolver_los_datos_de_PRD_001()
    {
        var error = ErrorCatalog.Find("PRD-001");

        Assert.Equal("PRD-001", error.Code);
        Assert.Equal(404, error.StatusCode);
        Assert.Equal("Producto no encontrado.", error.Message);
    }

    [Fact]
    public void Find_debe_devolver_error_de_regla_para_ORD_005()
    {
        var error = ErrorCatalog.Find("ORD-005");

        Assert.Equal(422, error.StatusCode);
        Assert.Equal(ErrorCategory.BusinessRule, error.Category);
    }

    [Fact]
    public void DomainException_debe_usar_los_metadatos_del_catálogo()
    {
        var exception = new DomainException("CRT-004");

        Assert.Equal("CRT-004", exception.ErrorCode);
        Assert.Equal(400, exception.StatusCode);
        Assert.Equal(ErrorCategory.Validation, exception.Category);
        Assert.Equal("Cantidad inválida.", exception.Message);
    }

    [Fact]
    public void ApiProblemDetails_debe_incluir_error_y_correlación()
    {
        var problem = new ApiProblemDetails(new DomainException("USR-003"), "correlation-123");

        Assert.Equal(401, problem.Status);
        Assert.Equal("USR-003", problem.ErrorCode);
        Assert.Equal("correlation-123", problem.CorrelationId);
    }
}
