using Products.API.DTOs;
using Products.API.Services;
using Shared.Contracts.Errors;

namespace Products.Tests;

public class ProductServiceTests
{
    [Fact]
    public void Create_debe_rechazar_nombre_duplicado_en_la_misma_categoría()
    {
        var service = new ProductService();
        var request = new CreateProductRequest("Notebook Dell", "Laptop", 1500, 10, "Electrónica");

        service.Create(request);

        var exception = Assert.Throws<DomainException>(() => service.Create(request));
        Assert.Equal("PRD-003", exception.ErrorCode);
    }
}
