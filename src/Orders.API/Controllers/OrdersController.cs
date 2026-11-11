// [TP Microservicios E-Commerce v7] Expone los endpoints REST de Orders.API y delega las reglas de la consigna al servicio correspondiente.
using Microsoft.AspNetCore.Mvc;
using Orders.API.DTOs;
using Orders.API.Services;

namespace Orders.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController(OrderService orderService) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyCollection<OrderResponse>> GetAll([FromQuery] Guid? usuarioId) => Ok(orderService.GetAll(usuarioId));

    [HttpGet("{id:guid}")]
    public ActionResult<OrderResponse> GetById(Guid id) => Ok(orderService.GetById(id));

    [HttpGet("active-by-product/{productId:guid}")]
    public ActionResult<bool> HasActiveOrders(Guid productId) => Ok(orderService.HasActiveOrders(productId));

    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    public ActionResult<OrderResponse> Create(CreateOrderRequest request)
    {
        var order = orderService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    [HttpPut("{id:guid}/status")]
    public ActionResult<OrderResponse> UpdateStatus(Guid id, UpdateOrderStatusRequest request) => Ok(orderService.UpdateStatus(id, request));
}
