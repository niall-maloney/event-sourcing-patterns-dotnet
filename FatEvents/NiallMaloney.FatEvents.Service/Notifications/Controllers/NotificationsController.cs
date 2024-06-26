using MediatR;
using Microsoft.AspNetCore.Mvc;
using NiallMaloney.FatEvents.Service.Notifications.Controllers.Models;
using NiallMaloney.FatEvents.Service.Notifications.Queries;

namespace NiallMaloney.FatEvents.Service.Notifications.Controllers;

[ApiController]
[Route("notifications")]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{notificationId}")]
    public async Task<IActionResult> GetNotification([FromRoute] string notificationId)
    {
        var row = await _mediator.Send(new GetNotification(notificationId));
        if (row is null)
        {
            return NotFound();
        }

        return Ok(Notification.Map(row));
    }

    [HttpGet]
    public async Task<IActionResult> SearchNotifications(
        [FromQuery] string? type = null,
        [FromQuery] string? status = null)
    {
        var rows = await _mediator.Send(new SearchNotifications(type, status));
        return Ok(Notification.Map(rows));
    }
}