// [TP Microservicios E-Commerce v7] Implementa reglas de negocio de Notifications.API solicitadas por la consigna.
using Notifications.API.DTOs;
using Notifications.API.Clients;
using Notifications.API.Models;
using Shared.Contracts.Errors;
using Shared.Infrastructure.Persistence;

namespace Notifications.API.Services;

public class NotificationService
{
    private readonly IUsersClient _usersClient;
    private readonly IEntityStore<Notification> _store;
    private readonly List<Notification> _notifications;

    public NotificationService(IUsersClient usersClient, IEntityStore<Notification>? store = null)
    {
        _usersClient = usersClient;
        _store = store ?? new InMemoryEntityStore<Notification>();
        _notifications = _store.Load().ToList();
    }

    public NotificationResponse Send(SendNotificationRequest request)
    {
        // Mensaje y tipo se validan localmente antes de consultar Users.
        if (request.UsuarioId == Guid.Empty || string.IsNullOrWhiteSpace(request.Mensaje) || request.Mensaje.Trim().Length > 500)
        {
            throw new DomainException("NTF-002");
        }

        if (request.Tipo is not ("Email" or "Push" or "SMS"))
        {
            throw new DomainException("NTF-002");
        }

        if (!_usersClient.Exists(request.UsuarioId))
        {
            // Notifications no crea usuarios: sólo puede enviar a un destinatario que pertenece a Users.
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

        // El envío es simulado, pero se persiste su resultado para poder consultar historial por usuario.
        _notifications.Add(notification);
        _store.Save(_notifications);
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
