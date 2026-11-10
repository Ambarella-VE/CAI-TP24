using Notifications.API.DTOs;
using Notifications.API.Services;
namespace Notifications.Tests;
public class NotificationServiceTests
{
    [Fact]
    public void Send_debe_registrar_notificación_para_el_usuario()
    {
        var service = new NotificationService();
        var result = service.Send(new SendNotificationRequest(Guid.NewGuid(), "Orden confirmada", "Email"));
        Assert.Equal("Enviada", result.Estado);
    }
}
