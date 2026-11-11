// [TP Microservicios E-Commerce v7] Expone los endpoints REST de Notifications.API y delega las reglas de la consigna al servicio correspondiente.
using Microsoft.AspNetCore.Mvc;
using Notifications.API.DTOs;
using Notifications.API.Services;

namespace Notifications.API.Controllers;

/// <summary>Registra y consulta notificaciones simuladas para usuarios existentes.</summary>
[ApiController]
[Route("api/notifications")]
public class NotificationsController(NotificationService notificationService) : ControllerBase
{
        /// <summary>Simula el envío de Email, Push o SMS y guarda el resultado.</summary>
[HttpPost("send")]
    [ProducesResponseType(typeof(NotificationResponse), StatusCodes.Status201Created)]
    public ActionResult<NotificationResponse> Send(SendNotificationRequest request)
    {
        var notification = notificationService.Send(request);
        return StatusCode(StatusCodes.Status201Created, notification);
    }

        /// <summary>Devuelve el historial de notificaciones de un usuario.</summary>
[HttpGet("{usuarioId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<NotificationResponse>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyCollection<NotificationResponse>> GetByUser(Guid usuarioId)
    {
        return Ok(notificationService.GetByUser(usuarioId));
    }
}
