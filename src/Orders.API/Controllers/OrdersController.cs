// [TP Microservicios E-Commerce v7] Expone los endpoints REST de Orders.API y delega las reglas de la consigna al servicio correspondiente.
using Microsoft.AspNetCore.Mvc;
using Orders.API.DTOs;
using Orders.API.Services;

namespace Orders.API.Controllers;

/// <summary>Gestiona compras, sus importes históricos y transiciones de estado.</summary>
[ApiController]
[Route("api/orders")]
public class OrdersController(OrderService orderService) : ControllerBase
{
        /// <summary>Lista órdenes; opcionalmente limita el resultado a un usuario.</summary>
[HttpGet]
    public ActionResult<IReadOnlyCollection<OrderResponse>> GetAll([FromQuery] Guid? usuarioId) => Ok(orderService.GetAll(usuarioId));

    [HttpGet("{id:guid}")]
    public ActionResult<OrderResponse> GetById(Guid id) => Ok(orderService.GetById(id));

    [HttpGet("active-by-product/{productId:guid}")]
    public ActionResult<bool> HasActiveOrders(Guid productId) => Ok(orderService.HasActiveOrders(productId));

        /// <summary>Crea una orden después de validar usuario, productos y stock.</summary>
[HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    public ActionResult<OrderResponse> Create(CreateOrderRequest request)
    {
        var order = orderService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

        /// <summary>Aplica una transición de estado permitida por la regla de negocio.</summary>
[HttpPut("{id:guid}/status")]
    public ActionResult<OrderResponse> UpdateStatus(Guid id, UpdateOrderStatusRequest request) => Ok(orderService.UpdateStatus(id, request));
}
