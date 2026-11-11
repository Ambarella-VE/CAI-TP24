// [TP Microservicios E-Commerce v7] Define el modelo interno de Cart.API; no se comparte como contrato entre microservicios.
namespace Cart.API.Models;
public class Cart { public Guid UserId { get; init; } public List<CartItem> Items { get; } = []; public DateTime UpdatedAt { get; set; } }
public class CartItem { public Guid ProductId { get; init; } public int Quantity { get; set; } }
