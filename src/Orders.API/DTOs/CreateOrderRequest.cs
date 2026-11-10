namespace Orders.API.DTOs;

public record CreateOrderItemRequest(Guid ProductoId, int Cantidad);
public record CreateOrderRequest(Guid UsuarioId, IReadOnlyCollection<CreateOrderItemRequest> Items);
public record UpdateOrderStatusRequest(string Estado);
public record OrderItemResponse(Guid ProductoId, int Cantidad, decimal PrecioUnitario);
public record OrderResponse(Guid Id, Guid UsuarioId, IReadOnlyCollection<OrderItemResponse> Items, decimal Total, string Estado, DateTime FechaCreacion);
