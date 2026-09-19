using AliRoyalMarquee.Application.Expenses.Commands.RecordExpense;
using AliRoyalMarquee.Application.Expenses.Queries.GetExpenses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AliRoyalMarquee.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExpensesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ExpensesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetExpenses()
    {
        var result = await _mediator.Send(new GetExpensesQuery());
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> RecordExpense([FromBody] RecordExpenseCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { id });
    }
}
