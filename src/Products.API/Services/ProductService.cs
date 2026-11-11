// [TP Microservicios E-Commerce v7] Implementa reglas de negocio de Products.API solicitadas por la consigna.
using Products.API.Clients;
using Products.API.DTOs;
using Products.API.Models;
using Shared.Contracts.Errors;
using Shared.Infrastructure.Persistence;

namespace Products.API.Services;

public class ProductService
{
    private readonly List<Product> _products;
    private readonly IOrdersClient _ordersClient;
    private readonly IEntityStore<Product> _store;

    public ProductService(IOrdersClient ordersClient, IEntityStore<Product>? store = null)
    {
        _ordersClient = ordersClient;
        _store = store ?? new InMemoryEntityStore<Product>();
        _products = _store.Load().ToList();
    }

    public ProductResponse Create(CreateProductRequest request)
    {
        // Se valida antes de normalizar para no guardar valores que contradigan el modelo del Apéndice A.
        Validate(request.Nombre, request.Descripcion, request.Precio, request.Stock, request.Categoria);
        var name = Normalize(request.Nombre);
        var category = Normalize(request.Categoria);

        if (_products.Any(product => SameText(product.Name, name) && SameText(product.Category, category)))
        {
            // El nombre sólo es único dentro de una categoría, tal como define PRD-003.
            throw DomainException.FromCode("PRD-003");
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = request.Descripcion?.Trim(),
            Price = request.Precio,
            Stock = request.Stock,
            Category = category,
            CreatedAt = DateTime.UtcNow
        };

        // La fecha se guarda en UTC para que la API no dependa de la zona horaria de quien la ejecute.
        _products.Add(product);
        _store.Save(_products);
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
        Validate(request.Nombre, request.Descripcion, request.Precio, request.Stock, request.Categoria);
        var product = FindProduct(id);
        var name = Normalize(request.Nombre);
        var category = Normalize(request.Categoria);

        if (_products.Any(other => other.Id != id && SameText(other.Name, name) && SameText(other.Category, category)))
        {
            throw DomainException.FromCode("PRD-003");
        }

        product.Name = name;
        product.Description = request.Descripcion?.Trim();
        product.Price = request.Precio;
        product.Stock = request.Stock;
        product.Category = category;
        _store.Save(_products);
        return ToResponse(product);
    }

    public void Delete(Guid id)
    {
        var product = FindProduct(id);

        if (_ordersClient.HasActiveOrders(id))
        {
            // Un producto referido por una orden en curso no se elimina para preservar la consistencia del historial.
            throw DomainException.FromCode("PRD-004");
        }

        _products.Remove(product);
        _store.Save(_products);
    }

    private Product FindProduct(Guid id)
    {
        // Centralizar la búsqueda evita que cada endpoint devuelva un error distinto para el mismo caso.
        return _products.FirstOrDefault(product => product.Id == id) ?? throw DomainException.FromCode("PRD-001");
    }

    private static void Validate(string name, string? description, decimal price, int stock, string category)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Trim().Length > 100 ||
            (description is not null && description.Trim().Length > 500) ||
            string.IsNullOrWhiteSpace(category) || price <= 0 || stock < 0)
        {
            throw DomainException.FromCode("PRD-002");
        }
    }

    private static string Normalize(string value) => value.Trim();

    private static bool SameText(string first, string second) => string.Equals(first.Trim(), second.Trim(), StringComparison.OrdinalIgnoreCase);

    private static ProductResponse ToResponse(Product product) => new(product.Id, product.Name, product.Description, product.Price, product.Stock, product.Category, product.CreatedAt);

}
