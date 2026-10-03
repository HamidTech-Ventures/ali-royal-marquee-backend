using System;
using System.Threading;
using System.Threading.Tasks;
using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using MediatR;

namespace AliRoyalMarquee.Application.Inventory.Commands
{
    public class AddInventoryReservationCommand : IRequest<Guid>
    {
        public Guid InventoryItemId { get; set; }
        public Guid EventId { get; set; }
        public decimal Quantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
    }

    public class AddInventoryReservationCommandHandler : IRequestHandler<AddInventoryReservationCommand, Guid>
    {
        private readonly IAppDbContext _context;
        public AddInventoryReservationCommandHandler(IAppDbContext context) { _context = context; }

        public async Task<Guid> Handle(AddInventoryReservationCommand request, CancellationToken cancellationToken)
        {
            var item = await _context.InventoryItems.FindAsync(new object[] { request.InventoryItemId }, cancellationToken);
            if (item == null) throw new Exception("Item not found");

            var reservation = new InventoryReservation
            {
                InventoryItemId = request.InventoryItemId,
                EventId = request.EventId,
                Quantity = request.Quantity,
                StartDate = request.StartDate.ToUniversalTime(),
                EndDate = request.EndDate.ToUniversalTime(),
                Status = request.Status
            };

            _context.InventoryReservations.Add(reservation);
            await _context.SaveChangesAsync(cancellationToken);
            return reservation.Id;
        }
    }
}
