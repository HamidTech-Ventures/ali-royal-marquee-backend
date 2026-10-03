import os

ctrl_path = r"c:\My working\HamidTech_Ventures\Clients\marquee-management-system\backend\src\AliRoyalMarquee.API\Controllers\InventoryController.cs"
with open(ctrl_path, "r") as f:
    code = f.read()

new_endpoints = """
        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryItemDto>> GetInventoryItemById(Guid id)
        {
            var item = await _mediator.Send(new GetInventoryItemByIdQuery { Id = id });
            if (item == null) return NotFound();
            return item;
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
"""
code = code.replace("        [HttpDelete(\"{id}\")]", new_endpoints + "\n        [HttpDelete(\"{id}\")]")

with open(ctrl_path, "w") as f:
    f.write(code)

print("Controller updated.")
