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
}
