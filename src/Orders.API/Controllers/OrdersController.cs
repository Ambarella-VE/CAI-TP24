using Microsoft.AspNetCore.Mvc;
using Orders.API.DTOs;
using Orders.API.Services;

namespace Orders.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController(OrderService orderService) : ControllerBase
{
    [HttpGet]
    public ActionResult<IReadOnlyCollection<OrderResponse>> GetAll() => Ok(orderService.GetAll());

    [HttpGet("{id:guid}")]
    public ActionResult<OrderResponse> GetById(Guid id) => Ok(orderService.GetById(id));

    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    public ActionResult<OrderResponse> Create(CreateOrderRequest request)
    {
        var order = orderService.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    [HttpPatch("{id:guid}/status")]
    public ActionResult<OrderResponse> UpdateStatus(Guid id, UpdateOrderStatusRequest request) => Ok(orderService.UpdateStatus(id, request));
}
