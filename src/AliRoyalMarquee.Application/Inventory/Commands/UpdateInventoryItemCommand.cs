using System;
using System.Threading;
using System.Threading.Tasks;
using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using MediatR;

namespace AliRoyalMarquee.Application.Inventory.Commands
{
    public class UpdateInventoryItemCommand : IRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Quantity { get; set; }
        public decimal MinQuantity { get; set; }
        public string Unit { get; set; }
        public string ItemType { get; set; }
    }

    public class UpdateInventoryItemCommandHandler : IRequestHandler<UpdateInventoryItemCommand>
    {
        private readonly IAppDbContext _context;

        public UpdateInventoryItemCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateInventoryItemCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.InventoryItems.FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity == null)
            {
                throw new Exception($"Entity {nameof(InventoryItem)} ({request.Id}) was not found.");
            }

            entity.Name = request.Name;
            entity.Category = request.Category;
            entity.Quantity = request.Quantity;
            entity.MinQuantity = request.MinQuantity;
            entity.Unit = request.Unit;
            entity.ItemType = request.ItemType;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
