using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AliRoyalMarquee.Application.StaffMembers.Commands;
using AliRoyalMarquee.Application.StaffMembers.DTOs;
using AliRoyalMarquee.Application.StaffMembers.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AliRoyalMarquee.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StaffController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<StaffDto>>> GetStaff()
        {
            return await _mediator.Send(new GetStaffQuery());
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateStaff(CreateStaffCommand command)
        {
            return await _mediator.Send(command);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateStaff(Guid id, UpdateStaffCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteStaff(Guid id)
        {
            await _mediator.Send(new DeleteStaffCommand { Id = id });
            return NoContent();
        }
    }
}
