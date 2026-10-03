using AliRoyalMarquee.Application.Customers.Commands.CreateCustomer;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AliRoyalMarquee.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetCustomer), new { id }, id);
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomers()
    {
        var result = await _mediator.Send(new Application.Customers.Queries.GetCustomers.GetCustomersQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomer(Guid id)
    {
        var result = await _mediator.Send(new Application.Customers.Queries.GetCustomerById.GetCustomerByIdQuery(id));
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] Application.Customers.Commands.UpdateCustomer.UpdateCustomerCommand command)
    {
        if (id != command.Id) return BadRequest();
        await _mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(Guid id)
    {
        await _mediator.Send(new Application.Customers.Commands.DeleteCustomer.DeleteCustomerCommand { Id = id });
        return NoContent();
    }
}
