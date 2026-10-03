using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Application.Inventory.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AliRoyalMarquee.Application.Inventory.Queries
{
    public class GetInventoryItemsQuery : IRequest<List<InventoryItemDto>>
    {
    }

    public class GetInventoryItemsQueryHandler : IRequestHandler<GetInventoryItemsQuery, List<InventoryItemDto>>
    {
        private readonly IAppDbContext _context;

        public GetInventoryItemsQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<InventoryItemDto>> Handle(GetInventoryItemsQuery request, CancellationToken cancellationToken)
        {
            return await _context.InventoryItems
                .AsNoTracking()
                .Select(x => new InventoryItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Category = x.Category,
                    Quantity = x.Quantity,
                    MinQuantity = x.MinQuantity,
                    Unit = x.Unit,
                    UnitPrice = x.UnitPrice,
                    Location = x.Location,
                    ItemType = string.IsNullOrEmpty(x.ItemType) ? "Fixed Asset" : x.ItemType
                })
                .ToListAsync(cancellationToken);
        }
    }
}
