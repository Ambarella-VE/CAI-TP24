// [TP Microservicios E-Commerce v7] Define el contrato JSON público de Products.API para requests y responses.
namespace Products.API.DTOs;

public record ProductResponse(Guid Id, string Nombre, string? Descripcion, decimal Precio, int Stock, string Categoria, DateTime FechaCreacion);
