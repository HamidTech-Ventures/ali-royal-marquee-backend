using AliRoyalMarquee.Application.Bookings.Commands.CreateBooking;
using AliRoyalMarquee.Application.Bookings.Queries.CheckAvailability;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AliRoyalMarquee.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BookingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetBooking), new { id }, id);
    }

    [HttpGet]
    public async Task<IActionResult> GetBookings([FromQuery] AliRoyalMarquee.Application.Bookings.Queries.GetBookings.GetBookingsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetBooking(Guid id)
    {
        var result = await _mediator.Send(new AliRoyalMarquee.Application.Bookings.Queries.GetBookingById.GetBookingByIdQuery(id));
        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateBooking(Guid id, [FromBody] AliRoyalMarquee.Application.Bookings.Commands.UpdateBooking.UpdateBookingCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch");
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] AliRoyalMarquee.Application.Bookings.Commands.UpdateBookingStatus.UpdateBookingStatusCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch");
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpPost("{id:guid}/payments")]
    public async Task<IActionResult> AddPayment(Guid id, [FromBody] AliRoyalMarquee.Application.Bookings.Commands.AddPayment.AddPaymentCommand command)
    {
        if (id != command.BookingId) return BadRequest("ID mismatch");
        var paymentId = await _mediator.Send(command);
        return Ok(new { paymentId });
    }

    [HttpGet("availability")]
    public async Task<IActionResult> CheckAvailability([FromQuery] CheckAvailabilityQuery query)
    {
        var isAvailable = await _mediator.Send(query);
        return Ok(new { available = isAvailable });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBooking(Guid id)
    {
        await _mediator.Send(new AliRoyalMarquee.Application.Bookings.Commands.DeleteBooking.DeleteBookingCommand(id));
        return NoContent();
    }

    [HttpGet("{id:guid}/invoice")]
    public async Task<IActionResult> GenerateInvoice(Guid id)
    {
        var url = await _mediator.Send(new AliRoyalMarquee.Application.Bookings.Commands.GenerateInvoice.GenerateInvoiceCommand { BookingId = id });
        return Ok(new { url });
    }
}
