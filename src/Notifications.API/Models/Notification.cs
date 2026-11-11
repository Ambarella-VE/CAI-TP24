// [TP Microservicios E-Commerce v7] Define el modelo interno de Notifications.API; no se comparte como contrato entre microservicios.
namespace Notifications.API.Models;

public class Notification
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Message { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Status { get; init; } = "Sent";
    public DateTime SentAt { get; init; }
}
