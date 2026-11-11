// [TP Microservicios E-Commerce v7] Define el modelo interno de Products.API; no se comparte como contrato entre microservicios.
namespace Products.API.Models;

public class Product
{
    public Guid Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
