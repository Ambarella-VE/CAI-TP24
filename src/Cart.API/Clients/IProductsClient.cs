namespace Cart.API.Clients;
public record ProductAvailability(Guid Id, int Stock);
public interface IProductsClient { ProductAvailability? Get(Guid id); }
public class EmptyProductsClient : IProductsClient { public ProductAvailability? Get(Guid id) => null; }
