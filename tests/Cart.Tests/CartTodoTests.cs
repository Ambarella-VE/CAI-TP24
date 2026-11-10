using Cart.API.Clients;
using Cart.API.DTOs;
using Cart.API.Services;

namespace Cart.Tests;

public class CartServiceTests
{
    [Fact]
    public void AddItem_debe_crear_el_carrito_y_sumar_items_repetidos()
    {
        var productId = Guid.NewGuid();
        var service = new CartService(new AvailableProductsClient(productId, 10));
        var userId = Guid.NewGuid();

        service.AddItem(userId, new AddCartItemRequest(productId, 2));
        var cart = service.AddItem(userId, new AddCartItemRequest(productId, 3));

        Assert.Single(cart.Items);
        Assert.Equal(5, cart.Items.Single().Cantidad);
    }

    private sealed class AvailableProductsClient(Guid productId, int stock) : IProductsClient
    {
        public ProductAvailability? Get(Guid id) => id == productId ? new ProductAvailability(id, stock) : null;
    }
}
