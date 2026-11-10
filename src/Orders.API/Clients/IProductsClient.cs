namespace Orders.API.Clients;

public record ProductForOrder(Guid Id, decimal Price, int Stock);

public interface IProductsClient
{
    ProductForOrder? Get(Guid productId);
}

public class EmptyProductsClient : IProductsClient
{
    public ProductForOrder? Get(Guid productId) => null;
}
