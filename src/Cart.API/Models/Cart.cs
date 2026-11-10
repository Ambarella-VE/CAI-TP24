namespace Cart.API.Models;
public class Cart { public Guid UserId { get; init; } public List<CartItem> Items { get; } = []; public DateTime UpdatedAt { get; set; } }
public class CartItem { public Guid ProductId { get; init; } public int Quantity { get; set; } }
