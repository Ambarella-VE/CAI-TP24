// [TP Microservicios E-Commerce v7] Configura o respalda una parte necesaria de Products.Tests para cumplir la consigna.
using Products.API.DTOs;
using Products.API.Clients;
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

    [Fact]
    public void GetAll_debe_combinar_filtros_sin_distinguir_mayúsculas()
    {
        var service = new ProductService();
        service.Create(new CreateProductRequest("Notebook Dell", null, 1500, 10, "Electrónica"));
        service.Create(new CreateProductRequest("Notebook Asus", null, 1200, 5, "Electrónica"));

        var result = service.GetAll("electrónica", "dell");

        Assert.Single(result);
        Assert.Equal("Notebook Dell", result.Single().Nombre);
    }

    [Fact]
    public void Delete_debe_rechazar_producto_con_órdenes_activas()
    {
        var service = new ProductService(new OrdersClientWithActiveOrders());
        var product = service.Create(new CreateProductRequest("Notebook Dell", null, 1500, 10, "Electrónica"));

        var exception = Assert.Throws<DomainException>(() => service.Delete(product.Id));

        Assert.Equal("PRD-004", exception.ErrorCode);
    }

    private sealed class OrdersClientWithActiveOrders : IOrdersClient
    {
        public bool HasActiveOrders(Guid productId) => true;
    }
}
