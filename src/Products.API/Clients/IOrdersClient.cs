namespace Products.API.Clients;

public interface IOrdersClient
{
    bool HasActiveOrders(Guid productId);
}
