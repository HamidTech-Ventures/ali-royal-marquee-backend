using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Application.Inventory.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AliRoyalMarquee.Application.Inventory.Queries
{
    public class GetInventoryItemByIdQuery : IRequest<InventoryItemDto>
    {
        public Guid Id { get; set; }
    }

    public class GetInventoryItemByIdQueryHandler : IRequestHandler<GetInventoryItemByIdQuery, InventoryItemDto>
    {
        private readonly IAppDbContext _context;
        public GetInventoryItemByIdQueryHandler(IAppDbContext context) { _context = context; }

        public async Task<InventoryItemDto> Handle(GetInventoryItemByIdQuery request, CancellationToken cancellationToken)
        {
            var item = await _context.InventoryItems
                .Include(i => i.Movements)
                .Include(i => i.Reservations)
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (item == null) return null;

            return new InventoryItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Category = item.Category,
                Quantity = item.Quantity,
                MinQuantity = item.MinQuantity,
                Unit = item.Unit,
                ItemType = string.IsNullOrEmpty(item.ItemType) ? "Fixed Asset" : item.ItemType,
                UnitPrice = item.UnitPrice,
                Location = item.Location,
                Movements = item.Movements.OrderByDescending(m => m.CreatedAt).Select(m => new InventoryMovementDto {
                    Id = m.Id, Type = m.Type, Quantity = m.Quantity, Notes = m.Notes, Reference = m.Reference, CreatedAt = m.CreatedAt
                }).ToList(),
                Reservations = item.Reservations.OrderByDescending(r => r.CreatedAt).Select(r => new InventoryReservationDto {
                    Id = r.Id, EventId = r.EventId, Quantity = r.Quantity, StartDate = r.StartDate, EndDate = r.EndDate, Status = r.Status, CreatedAt = r.CreatedAt
                }).ToList()
            };
        }
    }
}
