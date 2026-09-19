using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AliRoyalMarquee.Application.Vendors.Commands;
using AliRoyalMarquee.Application.Vendors.DTOs;
using AliRoyalMarquee.Application.Vendors.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AliRoyalMarquee.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendorsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VendorsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<VendorDto>>> GetVendors()
        {
            return await _mediator.Send(new GetVendorsQuery());
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateVendor(CreateVendorCommand command)
        {
            return await _mediator.Send(command);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateVendor(Guid id, UpdateVendorCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            await _mediator.Send(command);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteVendor(Guid id)
        {
            await _mediator.Send(new DeleteVendorCommand { Id = id });

            return NoContent();
        }
    }
}
