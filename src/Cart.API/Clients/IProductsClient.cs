// [TP Microservicios E-Commerce v7] Define la comunicación de Cart.API con otro microservicio sin compartir modelos internos.
namespace Cart.API.Clients;
public record ProductAvailability(Guid Id, int Stock);
public interface IProductsClient { ProductAvailability? Get(Guid id); }

public class ProductsHttpClient(HttpClient httpClient) : IProductsClient
{
    public ProductAvailability? Get(Guid id)
    {
        try
        {
            var product = httpClient.GetFromJsonAsync<ProductResponse>($"api/products/{id}").GetAwaiter().GetResult();
            return product is null ? null : new ProductAvailability(product.Id, product.Stock);
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }

    private record ProductResponse(Guid Id, int Stock);
}
