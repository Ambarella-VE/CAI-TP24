// [TP Microservicios E-Commerce v7] Configura o respalda una parte necesaria de Notifications.Tests para cumplir la consigna.
using Notifications.API.DTOs;
using Notifications.API.Clients;
using Notifications.API.Services;
using Shared.Contracts.Errors;
namespace Notifications.Tests;
public class NotificationServiceTests
{
    [Fact]
    public void Send_debe_registrar_notificación_para_el_usuario()
    {
        var service = new NotificationService(new ExistingUser());
        var result = service.Send(new SendNotificationRequest(Guid.NewGuid(), "Orden confirmada", "Email"));
        Assert.Equal("Enviada", result.Estado);
    }

    [Fact]
    public void Send_debe_devolver_NTF_001_si_el_usuario_no_existe()
    {
        var service = new NotificationService(new MissingUser());

        var exception = Assert.Throws<DomainException>(() => service.Send(new SendNotificationRequest(Guid.NewGuid(), "Orden confirmada", "Email")));

        Assert.Equal("NTF-001", exception.ErrorCode);
    }

    [Fact]
    public void Send_debe_devolver_NTF_002_para_un_tipo_invalido()
    {
        var service = new NotificationService(new ExistingUser());

        var exception = Assert.Throws<DomainException>(() => service.Send(new SendNotificationRequest(Guid.NewGuid(), "Orden confirmada", "Carta")));

        Assert.Equal("NTF-002", exception.ErrorCode);
    }

    [Fact]
    public void Send_debe_devolver_NTF_002_para_mensaje_mayor_a_500_caracteres()
    {
        var service = new NotificationService(new ExistingUser());

        var exception = Assert.Throws<DomainException>(() => service.Send(new SendNotificationRequest(Guid.NewGuid(), new string('a', 501), "Email")));

        Assert.Equal("NTF-002", exception.ErrorCode);
    }

    private sealed class ExistingUser : IUsersClient { public bool Exists(Guid userId) => true; }
    private sealed class MissingUser : IUsersClient { public bool Exists(Guid userId) => false; }
}
