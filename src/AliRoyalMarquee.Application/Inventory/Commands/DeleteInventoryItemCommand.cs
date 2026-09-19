using System;
using System.Threading;
using System.Threading.Tasks;
using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using MediatR;

namespace AliRoyalMarquee.Application.Inventory.Commands
{
    public class DeleteInventoryItemCommand : IRequest
    {
        public Guid Id { get; set; }
    }

    public class DeleteInventoryItemCommandHandler : IRequestHandler<DeleteInventoryItemCommand>
    {
        private readonly IAppDbContext _context;

        public DeleteInventoryItemCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteInventoryItemCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.InventoryItems.FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity == null)
            {
                throw new Exception($"Entity {nameof(InventoryItem)} ({request.Id}) was not found.");
            }

            _context.InventoryItems.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
