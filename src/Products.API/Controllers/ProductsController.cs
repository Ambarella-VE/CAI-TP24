// [TP Microservicios E-Commerce v7] Expone los endpoints REST de Products.API y delega las reglas de la consigna al servicio correspondiente.
using Microsoft.AspNetCore.Mvc;
using Products.API.DTOs;
using Products.API.Services;

namespace Products.API.Controllers;

/// <summary>Administra el catálogo y el stock de productos del e-commerce.</summary>
[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ProductService _service;

    public ProductsController(ProductService service)
    {
        _service = service;
    }

        /// <summary>Lista productos y permite filtrar por categoría o parte del nombre.</summary>
[HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<ProductResponse>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyCollection<ProductResponse>> GetAll([FromQuery] string? categoria, [FromQuery] string? nombre)
    {
        return Ok(_service.GetAll(categoria, nombre));
    }

        /// <summary>Obtiene un producto por su identificador.</summary>
[HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    public ActionResult<ProductResponse> GetById(Guid id)
    {
        return Ok(_service.GetById(id));
    }

        /// <summary>Crea un producto con precio y stock válidos.</summary>
[HttpPost]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    public ActionResult<ProductResponse> Create(CreateProductRequest request)
    {
        var product = _service.Create(request);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

        /// <summary>Actualiza los datos editables de un producto existente.</summary>
[HttpPut("{id:guid}")]
    public ActionResult<ProductResponse> Update(Guid id, UpdateProductRequest request)
    {
        return Ok(_service.Update(id, request));
    }

        /// <summary>Elimina un producto si no participa de órdenes activas.</summary>
[HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        _service.Delete(id);
        return NoContent();
    }
}
