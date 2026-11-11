// [TP Microservicios E-Commerce v7] Define el contrato JSON público de Cart.API para requests y responses.
namespace Cart.API.DTOs;
public record AddCartItemRequest(Guid ProductoId, int Cantidad);
public record UpdateCartItemRequest(int Cantidad);
public record CartItemResponse(Guid ProductoId, int Cantidad);
public record CartResponse(Guid UsuarioId, IReadOnlyCollection<CartItemResponse> Items, DateTime FechaActualizacion);
