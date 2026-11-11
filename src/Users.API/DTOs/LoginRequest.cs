// [TP Microservicios E-Commerce v7] Define el contrato JSON público de Users.API para requests y responses.
namespace Users.API.DTOs;
public record LoginRequest(string Email, string Password);
