// [TP Microservicios E-Commerce v7] Configura o respalda una parte necesaria de Users.Tests para cumplir la consigna.
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

        var exception = Assert.ThrowsAny<DomainException>(() => service.Register(request));
        Assert.Equal("USR-001", exception.ErrorCode);
    }

    [Fact]
    public void Register_debe_rechazar_email_con_formato_invalido()
    {
        var service = new UserService();

        var exception = Assert.ThrowsAny<DomainException>(() => service.Register(new RegisterUserRequest("Ana", "Pérez", "correo-invalido", "Clave123")));

        Assert.Equal("USR-002", exception.ErrorCode);
    }
}
