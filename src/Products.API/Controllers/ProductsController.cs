using Microsoft.AspNetCore.Mvc;
using Products.API.DTOs;
using Products.API.Services;

namespace Products.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _service;

    public ProductsController(ProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult<IReadOnlyCollection<ProductResponse>> GetAll([FromQuery] string? categoria, [FromQuery] string? nombre)
    {
        return Ok(_service.GetAll(categoria, nombre));
    }

    [HttpGet("{id:guid}")]
    public ActionResult<ProductResponse> GetById(Guid id)
    {
        return Ok(_service.GetById(id));
    }

    [HttpPost]
    public ActionResult<ProductResponse> Create(CreateProductRequest request)
    {
        var product = _service.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id:guid}")]
    public ActionResult<ProductResponse> Update(Guid id, UpdateProductRequest request)
    {
        return Ok(_service.Update(id, request));
    }

    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        _service.Delete(id);
        return NoContent();
    }
}
