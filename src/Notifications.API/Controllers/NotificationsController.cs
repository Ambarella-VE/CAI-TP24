using Microsoft.AspNetCore.Mvc;
using Notifications.API.DTOs;
using Notifications.API.Services;

namespace Notifications.API.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationsController(NotificationService notificationService) : ControllerBase
{
    [HttpPost("send")]
    [ProducesResponseType(typeof(NotificationResponse), StatusCodes.Status201Created)]
    public ActionResult<NotificationResponse> Send(SendNotificationRequest request)
    {
        var notification = notificationService.Send(request);
        return StatusCode(StatusCodes.Status201Created, notification);
    }

    [HttpGet("{usuarioId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyCollection<NotificationResponse>), StatusCodes.Status200OK)]
    public ActionResult<IReadOnlyCollection<NotificationResponse>> GetByUser(Guid usuarioId)
    {
        return Ok(notificationService.GetByUser(usuarioId));
    }
}
