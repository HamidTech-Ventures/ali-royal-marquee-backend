using AliRoyalMarquee.Application.Users.Queries.GetStaff;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AliRoyalMarquee.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("staff")]
    [ProducesResponseType(typeof(List<StaffDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStaff()
    {
        var result = await _mediator.Send(new GetStaffQuery());
        return Ok(result);
    }
}
