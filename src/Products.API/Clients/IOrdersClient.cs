// [TP Microservicios E-Commerce v7] Define la comunicación de Products.API con otro microservicio sin compartir modelos internos.
namespace Products.API.Clients;

public interface IOrdersClient
{
    bool HasActiveOrders(Guid productId);
}

public class OrdersHttpClient(HttpClient httpClient) : IOrdersClient
{
    public bool HasActiveOrders(Guid productId)
    {
        try
        {
            // La respuesta es booleana porque Products no necesita conocer los detalles privados de Orders.
            return httpClient.GetFromJsonAsync<bool>($"api/orders/active-by-product/{productId}").GetAwaiter().GetResult();
        }
        catch (HttpRequestException)
        {
            // Una caída de Orders no debe permitir borrar un producto que quizá esté en una compra activa.
            return true;
        }
    }
}
