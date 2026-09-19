using AliRoyalMarquee.Application.Venues.Queries.GetVenues;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AliRoyalMarquee.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VenuesController : ControllerBase
{
    private readonly IMediator _mediator;

    public VenuesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<VenueDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVenues()
    {
        var result = await _mediator.Send(new GetVenuesQuery());
        return Ok(result);
    }
}
