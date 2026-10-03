using System;
using System.Threading;
using System.Threading.Tasks;
using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using MediatR;

namespace AliRoyalMarquee.Application.Inventory.Commands
{
    public class CreateInventoryItemCommand : IRequest<Guid>
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Quantity { get; set; }
        public decimal MinQuantity { get; set; }
        public string Unit { get; set; }
        public string ItemType { get; set; }
    }

    public class CreateInventoryItemCommandHandler : IRequestHandler<CreateInventoryItemCommand, Guid>
    {
        private readonly IAppDbContext _context;

        public CreateInventoryItemCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateInventoryItemCommand request, CancellationToken cancellationToken)
        {
            var entity = new InventoryItem
            {
                Name = request.Name,
                Category = request.Category,
                Quantity = request.Quantity,
                MinQuantity = request.MinQuantity,
                Unit = request.Unit,
                ItemType = request.ItemType
            };

            _context.InventoryItems.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}
