using Shared.Contracts.Errors;
using Users.API.DTOs;
using Users.API.Services;

namespace Users.Tests;

public class UserServiceTests
{
    [Fact]
    public void Register_debe_rechazar_un_email_repetido()
    {
        var service = new UserService();
        var request = new RegisterUserRequest("María", "González", "maria@email.com", "MiPassword123!");

        service.Register(request);

        var exception = Assert.Throws<DomainException>(() => service.Register(request));
        Assert.Equal("USR-001", exception.ErrorCode);
    }
}
