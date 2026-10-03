using AliRoyalMarquee.Application.Events.Commands.CreateEventFromBooking;
using AliRoyalMarquee.Application.Events.Commands.ManageEventMenu;
using AliRoyalMarquee.Application.Events.Commands.ManageEventStaff;
using AliRoyalMarquee.Application.Events.Commands.ManageEventTasks;
using AliRoyalMarquee.Application.Events.Commands.UpdateEvent;
using AliRoyalMarquee.Application.Events.Queries.GetEventById;
using AliRoyalMarquee.Application.Events.Queries.GetEvents;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AliRoyalMarquee.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventsController : ControllerBase
{
    [HttpPost("from-booking/{bookingId:guid}")]
    public async Task<IActionResult> CreateEventFromBooking(Guid bookingId)
    {
        var eventId = await _mediator.Send(new CreateEventFromBookingCommand(bookingId));
        return Ok(new { eventId });
    }

    private readonly IMediator _mediator;

    public EventsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetEvents([FromQuery] GetEventsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetEvent(Guid id)
    {
        var result = await _mediator.Send(new GetEventByIdQuery(id));
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] UpdateEventCommand command)
    {
        if (id != command.Id) return BadRequest();
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPost("{id:guid}/tasks")]
    public async Task<IActionResult> AddTask(Guid id, [FromBody] AddEventTaskCommand command)
    {
        if (id != command.EventId) return BadRequest();
        var taskId = await _mediator.Send(command);
        return Ok(new { taskId });
    }

    [HttpPut("{id:guid}/tasks/{taskId:guid}")]
    public async Task<IActionResult> UpdateTask(Guid id, Guid taskId, [FromBody] UpdateEventTaskCommand command)
    {
        if (taskId != command.TaskId) return BadRequest();
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPost("{id:guid}/staff")]
    public async Task<IActionResult> AddStaff(Guid id, [FromBody] AddEventStaffCommand command)
    {
        if (id != command.EventId) return BadRequest();
        var staffId = await _mediator.Send(command);
        return Ok(new { staffId });
    }

    [HttpDelete("{id:guid}/staff/{staffMemberId:guid}")]
    public async Task<IActionResult> RemoveStaff(Guid id, Guid staffMemberId)
    {
        var result = await _mediator.Send(new RemoveEventStaffCommand(id, staffMemberId));
        return Ok(new { success = result });
    }

    [HttpPost("{id:guid}/menu")]
    public async Task<IActionResult> AddMenu(Guid id, [FromBody] AddEventMenuItemCommand command)
    {
        if (id != command.EventId) return BadRequest();
        var menuId = await _mediator.Send(command);
        return Ok(new { menuId });
    }
}
