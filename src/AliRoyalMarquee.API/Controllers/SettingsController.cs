using AliRoyalMarquee.Application.Settings.Commands.UpdateSettings;
using AliRoyalMarquee.Application.Settings.DTOs;
using AliRoyalMarquee.Application.Settings.Queries.GetSettings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AliRoyalMarquee.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SettingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SettingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<SettingDto>>> GetSettings()
    {
        var settings = await _mediator.Send(new GetSettingsQuery());
        return Ok(settings);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateSettings([FromBody] List<SettingDto> settings)
    {
        await _mediator.Send(new UpdateSettingsCommand { Settings = settings });
        return NoContent();
    }
}
