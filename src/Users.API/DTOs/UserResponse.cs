// [TP Microservicios E-Commerce v7] Define el contrato JSON público de Users.API para requests y responses.
namespace Users.API.DTOs;
public record UserResponse(Guid Id, string Nombre, string Apellido, string Email, DateTime FechaRegistro, bool Activo);
