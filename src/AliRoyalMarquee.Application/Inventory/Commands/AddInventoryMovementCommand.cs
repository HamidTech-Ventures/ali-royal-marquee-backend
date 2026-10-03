using System;
using System.Threading;
using System.Threading.Tasks;
using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using MediatR;

namespace AliRoyalMarquee.Application.Inventory.Commands
{
    public class AddInventoryMovementCommand : IRequest<Guid>
    {
        public Guid InventoryItemId { get; set; }
        public string Type { get; set; }
        public decimal Quantity { get; set; }
        public string Notes { get; set; }
        public string Reference { get; set; }
    }

    public class AddInventoryMovementCommandHandler : IRequestHandler<AddInventoryMovementCommand, Guid>
    {
        private readonly IAppDbContext _context;

        public AddInventoryMovementCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(AddInventoryMovementCommand request, CancellationToken cancellationToken)
        {
            var item = await _context.InventoryItems.FindAsync(new object[] { request.InventoryItemId }, cancellationToken);
            if (item == null) throw new Exception("Item not found");

            var movement = new InventoryMovement
            {
                InventoryItemId = request.InventoryItemId,
                Type = request.Type,
                Quantity = request.Quantity,
                Notes = request.Notes,
                Reference = request.Reference
            };

            // Update item quantity
            if (request.Type == "IN") item.Quantity += request.Quantity;
            else if (request.Type == "OUT") item.Quantity -= request.Quantity;

            _context.InventoryMovements.Add(movement);
            await _context.SaveChangesAsync(cancellationToken);
            return movement.Id;
        }
    }
}
