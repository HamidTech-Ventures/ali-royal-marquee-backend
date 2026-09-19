using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AliRoyalMarquee.Application.Packages.Commands;
using AliRoyalMarquee.Application.Packages.Queries;
using AliRoyalMarquee.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

using AliRoyalMarquee.Application.MenuItems.Commands;
using AliRoyalMarquee.Application.MenuItems.Queries;
using AliRoyalMarquee.Application.Addons.Commands;
using AliRoyalMarquee.Application.Addons.Queries;
using AliRoyalMarquee.Application.PricingRules.Commands;
using AliRoyalMarquee.Application.PricingRules.Queries;

namespace AliRoyalMarquee.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PackagesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PackagesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // --- Packages ---

        [HttpGet]
        public async Task<ActionResult<List<Package>>> GetPackages()
        {
            var packages = await _mediator.Send(new GetPackagesQuery());
            return Ok(packages);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreatePackage([FromBody] CreatePackageCommand command)
        {
            var packageId = await _mediator.Send(command);
            return Ok(packageId);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Package>> GetPackageById(Guid id)
        {
            var package = await _mediator.Send(new GetPackageByIdQuery(id));
            if (package == null) return NotFound();
            return Ok(package);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePackage(Guid id, [FromBody] UpdatePackageCommand command)
        {
            if (id != command.Id) return BadRequest("ID mismatch");
            var result = await _mediator.Send(command);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePackage(Guid id)
        {
            var result = await _mediator.Send(new DeletePackageCommand(id));
            if (!result) return NotFound();
            return NoContent();
        }

        // --- Menu Items ---

        [HttpGet("menu-items")]
        public async Task<ActionResult<List<MenuItem>>> GetMenuItems()
        {
            var items = await _mediator.Send(new GetMenuItemsQuery());
            return Ok(items);
        }

        [HttpPost("menu-items")]
        public async Task<ActionResult<Guid>> CreateMenuItem([FromBody] CreateMenuItemCommand command)
        {
            var itemId = await _mediator.Send(command);
            return Ok(itemId);
        }

        [HttpDelete("menu-items/{id}")]
        public async Task<ActionResult> DeleteMenuItem(Guid id)
        {
            var result = await _mediator.Send(new DeleteMenuItemCommand(id));
            if (!result) return NotFound();
            return NoContent();
        }

        // --- Addons ---

        [HttpGet("addons")]
        public async Task<ActionResult<List<Addon>>> GetAddons()
        {
            var items = await _mediator.Send(new GetAddonsQuery());
            return Ok(items);
        }

        [HttpPost("addons")]
        public async Task<ActionResult<Guid>> CreateAddon([FromBody] CreateAddonCommand command)
        {
            var itemId = await _mediator.Send(command);
            return Ok(itemId);
        }

        [HttpDelete("addons/{id}")]
        public async Task<ActionResult> DeleteAddon(Guid id)
        {
            var result = await _mediator.Send(new DeleteAddonCommand(id));
            if (!result) return NotFound();
            return NoContent();
        }

        // --- Pricing Rules ---

        [HttpGet("pricing-rules")]
        public async Task<ActionResult<List<PricingRule>>> GetPricingRules()
        {
            var items = await _mediator.Send(new GetPricingRulesQuery());
            return Ok(items);
        }

        [HttpPost("pricing-rules")]
        public async Task<ActionResult<Guid>> CreatePricingRule([FromBody] CreatePricingRuleCommand command)
        {
            var itemId = await _mediator.Send(command);
            return Ok(itemId);
        }

        [HttpDelete("pricing-rules/{id}")]
        public async Task<ActionResult> DeletePricingRule(Guid id)
        {
            var result = await _mediator.Send(new DeletePricingRuleCommand(id));
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
