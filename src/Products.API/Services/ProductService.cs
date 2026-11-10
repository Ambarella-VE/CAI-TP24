using Products.API.Clients;
using Products.API.DTOs;
using Products.API.Models;
using Shared.Contracts.Errors;

namespace Products.API.Services;

public class ProductService
{
    private readonly List<Product> _products = [];
    private readonly IOrdersClient _ordersClient;

    public ProductService(IOrdersClient? ordersClient = null)
    {
        _ordersClient = ordersClient ?? new EmptyOrdersClient();
    }

    public ProductResponse Create(CreateProductRequest request)
    {
        Validate(request.Name, request.Price, request.Stock, request.Category);
        var name = Normalize(request.Name);
        var category = Normalize(request.Category);

        if (_products.Any(product => SameText(product.Name, name) && SameText(product.Category, category)))
        {
            throw new DomainException("PRD-003");
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = request.Description?.Trim(),
            Price = request.Price,
            Stock = request.Stock,
            Category = category,
            CreatedAt = DateTime.UtcNow
        };

        _products.Add(product);
        return ToResponse(product);
    }

    public ProductResponse GetById(Guid id)
    {
        return ToResponse(FindProduct(id));
    }

    public IReadOnlyCollection<ProductResponse> GetAll(string? category = null, string? name = null)
    {
        IEnumerable<Product> result = _products;

        if (!string.IsNullOrWhiteSpace(category))
        {
            result = result.Where(product => SameText(product.Category, category.Trim()));
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            result = result.Where(product => product.Name.Contains(name.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        return result.Select(ToResponse).ToList();
    }

    public ProductResponse Update(Guid id, UpdateProductRequest request)
    {
        Validate(request.Name, request.Price, request.Stock, request.Category);
        var product = FindProduct(id);
        var name = Normalize(request.Name);
        var category = Normalize(request.Category);

        if (_products.Any(other => other.Id != id && SameText(other.Name, name) && SameText(other.Category, category)))
        {
            throw new DomainException("PRD-003");
        }

        product.Name = name;
        product.Description = request.Description?.Trim();
        product.Price = request.Price;
        product.Stock = request.Stock;
        product.Category = category;
        return ToResponse(product);
    }

    public void Delete(Guid id)
    {
        var product = FindProduct(id);

        if (_ordersClient.HasActiveOrders(id))
        {
            throw new DomainException("PRD-004");
        }

        _products.Remove(product);
    }

    private Product FindProduct(Guid id)
    {
        return _products.FirstOrDefault(product => product.Id == id) ?? throw new DomainException("PRD-001");
    }

    private static void Validate(string name, decimal price, int stock, string category)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(category) || price <= 0 || stock < 0)
        {
            throw new DomainException("PRD-002");
        }
    }

    private static string Normalize(string value) => value.Trim();

    private static bool SameText(string first, string second) => string.Equals(first.Trim(), second.Trim(), StringComparison.OrdinalIgnoreCase);

    private static ProductResponse ToResponse(Product product) => new(product.Id, product.Name, product.Description, product.Price, product.Stock, product.Category, product.CreatedAt);

    private sealed class EmptyOrdersClient : IOrdersClient
    {
        public bool HasActiveOrders(Guid productId) => false;
    }
}
