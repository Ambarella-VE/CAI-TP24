// [TP Microservicios E-Commerce v7] Implementa reglas de negocio de Notifications.API solicitadas por la consigna.
using Notifications.API.DTOs;
using Notifications.API.Clients;
using Notifications.API.Models;
using Shared.Contracts.Errors;

namespace Notifications.API.Services;

public class NotificationService(IUsersClient usersClient)
{
    private readonly List<Notification> _notifications = [];

    public NotificationResponse Send(SendNotificationRequest request)
    {
        if (request.UsuarioId == Guid.Empty || string.IsNullOrWhiteSpace(request.Mensaje))
        {
            throw new DomainException("NTF-002");
        }

        if (request.Tipo is not ("Email" or "Push" or "SMS"))
        {
            throw new DomainException("NTF-002");
        }

        if (!usersClient.Exists(request.UsuarioId))
        {
            throw new DomainException("NTF-001");
        }

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            UserId = request.UsuarioId,
            Message = request.Mensaje.Trim(),
            Type = request.Tipo,
            SentAt = DateTime.UtcNow
        };

        _notifications.Add(notification);
        return ToResponse(notification);
    }

    public IReadOnlyCollection<NotificationResponse> GetByUser(Guid userId)
    {
        var notifications = _notifications
            .Where(notification => notification.UserId == userId)
            .Select(ToResponse)
            .ToList();

        if (notifications.Count == 0)
        {
            throw new DomainException("NTF003");
        }

        return notifications;
    }

    private static NotificationResponse ToResponse(Notification notification) => new(
        notification.Id,
        notification.UserId,
        notification.Message,
        notification.Type,
        "Enviada",
        notification.SentAt);
}
