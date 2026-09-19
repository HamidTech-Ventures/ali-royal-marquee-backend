using AliRoyalMarquee.Application.Payments.Commands.RecordPayment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AliRoyalMarquee.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> RecordPayment([FromBody] RecordPaymentCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetPayment), new { id }, id);
    }

    [HttpGet]
    public async Task<IActionResult> GetPayments()
    {
        var result = await _mediator.Send(new Application.Payments.Queries.GetPayments.GetPaymentsQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public IActionResult GetPayment(Guid id) => Ok();
}
