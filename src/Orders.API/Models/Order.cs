namespace Orders.API.Models;

public enum OrderStatus { Pending, Confirmed, Shipped, Delivered, Cancelled }

public class Order
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public List<OrderItem> Items { get; init; } = [];
    public decimal Total { get; init; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public DateTime CreatedAt { get; init; }
}

public class OrderItem
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}
