using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AliRoyalMarquee.Application.Inventory.Commands;
using AliRoyalMarquee.Application.Inventory.DTOs;
using AliRoyalMarquee.Application.Inventory.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AliRoyalMarquee.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InventoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<InventoryItemDto>>> GetInventoryItems()
        {
            return await _mediator.Send(new GetInventoryItemsQuery());
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateInventoryItem(CreateInventoryItemCommand command)
        {
            return await _mediator.Send(command);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateInventoryItem(Guid id, UpdateInventoryItemCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            await _mediator.Send(command);

            return NoContent();
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryItemDto>> GetInventoryItemById(Guid id)
        {
            var item = await _mediator.Send(new GetInventoryItemByIdQuery { Id = id });
            if (item == null) return NotFound();
            return item;
        }

        [HttpPost("reservations")]
        public async Task<ActionResult<Guid>> AddReservation(AddInventoryReservationCommand command)
        {
            return await _mediator.Send(command);
        }

        [HttpPost("movements")]
        public async Task<ActionResult<Guid>> AddMovement(AddInventoryMovementCommand command)
        {
            return await _mediator.Send(command);
        }

        [HttpPut("{id}/details")]
        public async Task<ActionResult> UpdateDetails(Guid id, UpdateInventoryItemDetailsCommand command)
        {
            if (id != command.Id) return BadRequest();
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteInventoryItem(Guid id)
        {
            await _mediator.Send(new DeleteInventoryItemCommand { Id = id });

            return NoContent();
        }
    }
}
