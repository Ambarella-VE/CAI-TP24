// [TP Microservicios E-Commerce v7] Expone los endpoints REST de Users.API y delega las reglas de la consigna al servicio correspondiente.
using Microsoft.AspNetCore.Mvc;
using Users.API.DTOs;
using Users.API.Services;

namespace Users.API.Controllers;

[ApiController]
[Route("api/users")]
/// <summary>Registra usuarios, autentica credenciales y protege datos sensibles.</summary>
public class UsersController : ControllerBase
{
    private readonly UserService _service;
    public UsersController(UserService service) => _service = service;

    [HttpPost("register")]
    /// <summary>Registra un usuario y guarda únicamente el hash de su contraseña.</summary>
    public ActionResult<UserResponse> Register(RegisterUserRequest request) => StatusCode(201, _service.Register(request));

    [HttpPost("login")]
    /// <summary>Autentica al usuario y aplica el bloqueo por intentos fallidos.</summary>
    public ActionResult<UserResponse> Login(LoginRequest request) => Ok(_service.Login(request));

    [HttpGet("{id:guid}")]
    /// <summary>Consulta interna utilizada por otros microservicios para validar un usuario.</summary>
    public ActionResult<UserResponse> GetById(Guid id) => Ok(_service.GetById(id));
}
