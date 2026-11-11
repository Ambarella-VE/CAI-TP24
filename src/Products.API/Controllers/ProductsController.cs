// [TP Microservicios E-Commerce v7] Expone los endpoints REST de Products.API y delega las reglas de la consigna al servicio correspondiente.
using Microsoft.AspNetCore.Mvc;
using Products.API.DTOs;
using Products.API.Services;

namespace Products.API.Controllers;

[ApiController]
[Route("api/products")]
/// <summary>Administra el catálogo y el stock de productos del e-commerce.</summary>
public class ProductsController : ControllerBase
{
    private readonly ProductService _service;

    public ProductsController(ProductService service)
    {
        _service = service;
    }

    [HttpGet]
    /// <summary>Lista productos y permite filtrar por categoría o parte del nombre.</summary>
    [ProducesResponseType(typeof(IReadOnlyCollection<ProductResponse>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyCollection<ProductResponse>> GetAll([FromQuery] string? categoria, [FromQuery] string? nombre)
    {
        return Ok(_service.GetAll(categoria, nombre));
    }

    [HttpGet("{id:guid}")]
    /// <summary>Obtiene un producto por su identificador.</summary>
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    public ActionResult<ProductResponse> GetById(Guid id)
    {
        return Ok(_service.GetById(id));
    }

    [HttpPost]
    /// <summary>Crea un producto con precio y stock válidos.</summary>
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    public ActionResult<ProductResponse> Create(CreateProductRequest request)
    {
        var product = _service.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id:guid}")]
    /// <summary>Actualiza los datos editables de un producto existente.</summary>
    public ActionResult<ProductResponse> Update(Guid id, UpdateProductRequest request)
    {
        return Ok(_service.Update(id, request));
    }

    [HttpDelete("{id:guid}")]
    /// <summary>Elimina un producto si no participa de órdenes activas.</summary>
    public IActionResult Delete(Guid id)
    {
        _service.Delete(id);
        return NoContent();
    }
}
