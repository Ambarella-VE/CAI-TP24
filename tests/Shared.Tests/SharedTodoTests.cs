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
}
