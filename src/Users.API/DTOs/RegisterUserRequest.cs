// [TP Microservicios E-Commerce v7] Define el contrato JSON público de Users.API para requests y responses.
namespace Users.API.DTOs;
public record RegisterUserRequest(string Nombre, string Apellido, string Email, string Password);
