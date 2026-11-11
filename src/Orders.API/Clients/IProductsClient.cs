// [TP Microservicios E-Commerce v7] Define la comunicación de Orders.API con otro microservicio sin compartir modelos internos.
namespace Orders.API.Clients;

public record ProductForOrder(Guid Id, decimal Price, int Stock);

public interface IProductsClient
{
    ProductForOrder? Get(Guid productId);
}

public class ProductsHttpClient(HttpClient httpClient) : IProductsClient
{
    public ProductForOrder? Get(Guid productId)
    {
        try
        {
            var product = httpClient.GetFromJsonAsync<ProductResponse>($"api/products/{productId}").GetAwaiter().GetResult();
            return product is null ? null : new ProductForOrder(product.Id, product.Precio, product.Stock);
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    private record ProductResponse(Guid Id, decimal Precio, int Stock);
}
