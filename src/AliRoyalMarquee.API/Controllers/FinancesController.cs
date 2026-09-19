using AliRoyalMarquee.Application.Finances.Queries.GetFinancialOverview;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AliRoyalMarquee.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FinancesController : ControllerBase
{
    private readonly IMediator _mediator;

    public FinancesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("overview")]
    public async Task<IActionResult> GetFinancialOverview()
    {
        var result = await _mediator.Send(new GetFinancialOverviewQuery());
        return Ok(result);
    }
}
