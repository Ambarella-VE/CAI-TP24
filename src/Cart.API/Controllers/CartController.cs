// [TP Microservicios E-Commerce v7] Expone los endpoints REST de Cart.API y delega las reglas de la consigna al servicio correspondiente.
using Cart.API.DTOs;
using Cart.API.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts.Errors;
namespace Cart.API.Controllers;
/// <summary>Gestiona el carrito de un usuario y valida stock con Products.</summary>
[ApiController]
[Route("api/cart")]
[ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status400BadRequest)]
[ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status404NotFound)]
[ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status422UnprocessableEntity)]
[ProducesResponseType(typeof(ApiProblemDetails), StatusCodes.Status500InternalServerError)]
public class CartController(CartService service) : ControllerBase
{
    /// <summary>Obtiene el carrito actual de un usuario.</summary>
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    public ActionResult<CartResponse> Get(Guid userId) => Ok(service.Get(userId));

    /// <summary>Agrega unidades de un producto al carrito.</summary>
    [HttpPost("{userId:guid}/items")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    public ActionResult<CartResponse> Add(Guid userId, AddCartItemRequest request) => Ok(service.AddItem(userId, request));

    /// <summary>Reemplaza la cantidad de un producto ya presente.</summary>
    [HttpPut("{userId:guid}/items/{productId:guid}")]
    [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
    public ActionResult<CartResponse> Update(Guid userId, Guid productId, UpdateCartItemRequest request) => Ok(service.UpdateItem(userId, productId, request));

    /// <summary>Quita una línea de producto del carrito.</summary>
    [HttpDelete("{userId:guid}/items/{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Delete(Guid userId, Guid productId) { service.DeleteItem(userId, productId); return NoContent(); }

    /// <summary>Vacía todas las líneas del carrito sin eliminar el recurso.</summary>
    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Clear(Guid userId) { service.Clear(userId); return NoContent(); }
}
