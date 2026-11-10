using Cart.API.DTOs;
using Cart.API.Services;
using Microsoft.AspNetCore.Mvc;
namespace Cart.API.Controllers;
[ApiController]
[Route("api/cart")]
public class CartController(CartService service) : ControllerBase
{
    [HttpGet("{userId:guid}")] public ActionResult<CartResponse> Get(Guid userId) => Ok(service.Get(userId));
    [HttpPost("{userId:guid}/items")] public ActionResult<CartResponse> Add(Guid userId, AddCartItemRequest request) => Ok(service.AddItem(userId, request));
    [HttpPut("{userId:guid}/items/{productId:guid}")] public ActionResult<CartResponse> Update(Guid userId, Guid productId, UpdateCartItemRequest request) => Ok(service.UpdateItem(userId, productId, request));
    [HttpDelete("{userId:guid}/items/{productId:guid}")] public IActionResult Delete(Guid userId, Guid productId) { service.DeleteItem(userId, productId); return NoContent(); }
    [HttpDelete("{userId:guid}")] public IActionResult Clear(Guid userId) { service.Clear(userId); return NoContent(); }
}
