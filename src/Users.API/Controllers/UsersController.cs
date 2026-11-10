using Microsoft.AspNetCore.Mvc;
using Users.API.DTOs;
using Users.API.Services;

namespace Users.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly UserService _service;
    public UsersController(UserService service) => _service = service;

    [HttpPost("register")]
    public ActionResult<UserResponse> Register(RegisterUserRequest request) => StatusCode(201, _service.Register(request));

    [HttpPost("login")]
    public ActionResult<UserResponse> Login(LoginRequest request) => Ok(_service.Login(request));
}
