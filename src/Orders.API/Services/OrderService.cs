// [TP Microservicios E-Commerce v7] Implementa reglas de negocio de Orders.API solicitadas por la consigna.
using Orders.API.Clients;
using Orders.API.DTOs;
using Orders.API.Models;
using Shared.Contracts.Errors;
using Shared.Infrastructure.Persistence;

namespace Orders.API.Services;

public class OrderService
{
    private readonly IUsersClient _usersClient;
    private readonly IProductsClient _productsClient;
    private readonly IEntityStore<Order> _store;
    private readonly List<Order> _orders;

    public OrderService(IUsersClient usersClient, IProductsClient productsClient, IEntityStore<Order>? store = null)
    {
        _usersClient = usersClient;
        _productsClient = productsClient;
        _store = store ?? new InMemoryEntityStore<Order>();
        _orders = _store.Load().ToList();
    }

    public OrderResponse Create(CreateOrderRequest request)
    {
        // Una orden vacía o con cantidades no positivas no puede representar una compra válida.
        if (request.UsuarioId == Guid.Empty || request.Items.Count == 0 || request.Items.Any(item => item.Cantidad <= 0))
        {
            throw new DomainException("ORD-002");
        }

        if (!_usersClient.Exists(request.UsuarioId))
        {
            throw new DomainException("ORD-003");
        }

        var items = new List<OrderItem>();
        // Se consolidan productos repetidos antes de consultar stock, para validar la cantidad real pedida.
        foreach (var requestedItem in request.Items.GroupBy(item => item.ProductoId))
        {
            var product = _productsClient.Get(requestedItem.Key) ?? throw new DomainException("ORD-004");
            var quantity = requestedItem.Sum(item => item.Cantidad);
            if (product.Stock < quantity)
            {
                throw new DomainException("ORD-005");
            }

            // El precio queda congelado en la orden aunque Products cambie el catálogo después.
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
        _store.Save(_orders);
        return ToResponse(order);
    }

    public OrderResponse GetById(Guid orderId) => ToResponse(FindOrder(orderId));

    public IReadOnlyCollection<OrderResponse> GetAll(Guid? userId = null)
    {
        var orders = userId.HasValue
            ? _orders.Where(order => order.UserId == userId.Value)
            : _orders;

        return orders.Select(ToResponse).ToList();
    }

    public bool HasActiveOrders(Guid productId)
    {
        // Products sólo necesita saber si puede borrar: Pendiente y Confirmada todavía protegen el producto.
        return _orders.Any(order => order.Items.Any(item => item.ProductId == productId) &&
            order.Status is OrderStatus.Pending or OrderStatus.Confirmed);
    }

    public OrderResponse UpdateStatus(Guid orderId, UpdateOrderStatusRequest request)
    {
        var order = FindOrder(orderId);
        if (!Enum.TryParse<OrderStatus>(request.Estado, true, out var nextStatus) ||
            !OrderStateTransitionPolicy.CanChange(order.Status, nextStatus))
        {
            // La matriz de transiciones evita saltos inválidos y mantiene los estados terminales.
            throw new DomainException("ORD-006");
        }

        order.Status = nextStatus;
        _store.Save(_orders);
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
