using Orders.API.Clients;
using Orders.API.DTOs;
using Orders.API.Services;
using Shared.Contracts.Errors;

namespace Orders.Tests;

public class OrderServiceTests
{
    [Fact]
    public void Create_debe_consolidar_items_repetidos_y_calcular_total()
    {
        var productId = Guid.NewGuid();
        var service = new OrderService(new ExistingUser(), new ProductClient(productId, 25m, 10));

        var order = service.Create(new CreateOrderRequest(Guid.NewGuid(), [
            new CreateOrderItemRequest(productId, 1),
            new CreateOrderItemRequest(productId, 2)
        ]));

        var item = Assert.Single(order.Items);
        Assert.Equal(3, item.Cantidad);
        Assert.Equal(75m, order.Total);
    }

    [Fact]
    public void UpdateStatus_debe_rechazar_salto_de_pendiente_a_enviada()
    {
        var productId = Guid.NewGuid();
        var service = new OrderService(new ExistingUser(), new ProductClient(productId, 10m, 2));
        var order = service.Create(new CreateOrderRequest(Guid.NewGuid(), [new CreateOrderItemRequest(productId, 1)]));

        var exception = Assert.Throws<DomainException>(() => service.UpdateStatus(order.Id, new UpdateOrderStatusRequest("Shipped")));

        Assert.Equal("ORD-006", exception.ErrorCode);
    }

    private class ExistingUser : IUsersClient { public bool Exists(Guid userId) => true; }
    private class ProductClient(Guid id, decimal price, int stock) : IProductsClient
    {
        public ProductForOrder? Get(Guid productId) => productId == id ? new ProductForOrder(id, price, stock) : null;
    }
}
