// [TP Microservicios E-Commerce v7] Expone los endpoints REST de Users.API y delega las reglas de la consigna al servicio correspondiente.
using Microsoft.AspNetCore.Mvc;
using Users.API.DTOs;
using Users.API.Services;

namespace Users.API.Controllers;

/// <summary>Registra usuarios, autentica credenciales y protege datos sensibles.</summary>
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly UserService _service;
    public UsersController(UserService service) => _service = service;

        /// <summary>Registra un usuario y guarda únicamente el hash de su contraseña.</summary>
[HttpPost("register")]
    public ActionResult<UserResponse> Register(RegisterUserRequest request) => StatusCode(201, _service.Register(request));

        /// <summary>Autentica al usuario y aplica el bloqueo por intentos fallidos.</summary>
[HttpPost("login")]
    public ActionResult<UserResponse> Login(LoginRequest request) => Ok(_service.Login(request));

        /// <summary>Consulta interna utilizada por otros microservicios para validar un usuario.</summary>
[HttpGet("{id:guid}")]
    public ActionResult<UserResponse> GetById(Guid id) => Ok(_service.GetById(id));
}
