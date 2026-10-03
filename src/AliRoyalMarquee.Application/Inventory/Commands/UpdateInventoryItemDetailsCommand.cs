using System;
using System.Threading;
using System.Threading.Tasks;
using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;

namespace AliRoyalMarquee.Application.Inventory.Commands
{
    public class UpdateInventoryItemDetailsCommand : IRequest
    {
        public Guid Id { get; set; }
        public decimal UnitPrice { get; set; }
        public string Location { get; set; }
    }

    public class UpdateInventoryItemDetailsCommandHandler : IRequestHandler<UpdateInventoryItemDetailsCommand>
    {
        private readonly IAppDbContext _context;
        public UpdateInventoryItemDetailsCommandHandler(IAppDbContext context) { _context = context; }

        public async Task Handle(UpdateInventoryItemDetailsCommand request, CancellationToken cancellationToken)
        {
            var item = await _context.InventoryItems.FindAsync(new object[] { request.Id }, cancellationToken);
            if (item != null)
            {
                item.UnitPrice = request.UnitPrice;
                item.Location = request.Location;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
