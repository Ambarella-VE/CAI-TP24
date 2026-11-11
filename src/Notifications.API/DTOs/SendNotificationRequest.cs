// [TP Microservicios E-Commerce v7] Define el contrato JSON público de Notifications.API para requests y responses.
namespace Notifications.API.DTOs;

public record SendNotificationRequest(Guid UsuarioId, string Mensaje, string Tipo);

public record NotificationResponse(
    Guid Id,
    Guid UsuarioId,
    string Mensaje,
    string Tipo,
    string Estado,
    DateTime FechaEnvio);
