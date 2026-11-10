using Orders.API.Clients;
using Orders.API.DTOs;
using Orders.API.Models;
using Shared.Contracts.Errors;

namespace Orders.API.Services;

public class OrderService(IUsersClient usersClient, IProductsClient productsClient)
{
    private readonly List<Order> _orders = [];

    public OrderResponse Create(CreateOrderRequest request)
    {
        if (request.UsuarioId == Guid.Empty || request.Items.Count == 0 || request.Items.Any(item => item.Cantidad <= 0))
        {
            throw new DomainException("ORD-002");
        }

        if (!usersClient.Exists(request.UsuarioId))
        {
            throw new DomainException("ORD-003");
        }

        var items = new List<OrderItem>();
        foreach (var requestedItem in request.Items.GroupBy(item => item.ProductoId))
        {
            var product = productsClient.Get(requestedItem.Key) ?? throw new DomainException("ORD-004");
            var quantity = requestedItem.Sum(item => item.Cantidad);
            if (product.Stock < quantity)
            {
                throw new DomainException("ORD-005");
            }

            items.Add(new OrderItem { ProductId = product.Id, Quantity = quantity, UnitPrice = product.Price });
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = request.UsuarioId,
            Items = items,
            Total = items.Sum(item => item.Quantity * item.UnitPrice),
            CreatedAt = DateTime.UtcNow
        };

        _orders.Add(order);
        return ToResponse(order);
    }

    public OrderResponse GetById(Guid orderId) => ToResponse(FindOrder(orderId));

    public IReadOnlyCollection<OrderResponse> GetAll() => _orders.Select(ToResponse).ToList();

    public OrderResponse UpdateStatus(Guid orderId, UpdateOrderStatusRequest request)
    {
        var order = FindOrder(orderId);
        if (!Enum.TryParse<OrderStatus>(request.Estado, true, out var nextStatus) ||
            !OrderStateTransitionPolicy.CanChange(order.Status, nextStatus))
        {
            throw new DomainException("ORD-006");
        }

        order.Status = nextStatus;
        return ToResponse(order);
    }

    private Order FindOrder(Guid orderId) => _orders.FirstOrDefault(order => order.Id == orderId) ?? throw new DomainException("ORD-001");

    private static OrderResponse ToResponse(Order order) => new(
        order.Id,
        order.UserId,
        order.Items.Select(item => new OrderItemResponse(item.ProductId, item.Quantity, item.UnitPrice)).ToList(),
        order.Total,
        ToSpanishStatus(order.Status),
        order.CreatedAt);

    private static string ToSpanishStatus(OrderStatus status) => status switch
    {
        OrderStatus.Pending => "Pendiente",
        OrderStatus.Confirmed => "Confirmada",
        OrderStatus.Shipped => "Enviada",
        OrderStatus.Delivered => "Entregada",
        _ => "Cancelada"
    };
}
