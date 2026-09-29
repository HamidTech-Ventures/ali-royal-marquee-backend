using AliRoyalMarquee.Application.Notifications.Commands.MarkAsRead;
using AliRoyalMarquee.Application.Notifications.DTOs;
using AliRoyalMarquee.Application.Notifications.Queries.GetNotifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AliRoyalMarquee.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<NotificationDto>>> GetNotifications()
    {
        // Get user ID from claims (mocking with a static Guid for now if not fully setup)
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        _ = Guid.TryParse(userIdString, out var userId);
        
        var notifications = await _mediator.Send(new GetNotificationsQuery { UserId = userId });
        return Ok(notifications);
    }

    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var result = await _mediator.Send(new MarkNotificationAsReadCommand { Id = id });
        if (!result) return NotFound();
        return NoContent();
    }
}
