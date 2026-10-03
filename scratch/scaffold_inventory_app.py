import os

app_dir = r"c:\My working\HamidTech_Ventures\Clients\marquee-management-system\backend\src\AliRoyalMarquee.Application\Inventory"

# DTOs
dtos_dir = os.path.join(app_dir, "DTOs")
os.makedirs(dtos_dir, exist_ok=True)

with open(os.path.join(dtos_dir, "InventoryMovementDto.cs"), "w") as f:
    f.write("""using System;
namespace AliRoyalMarquee.Application.Inventory.DTOs
{
    public class InventoryMovementDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public decimal Quantity { get; set; }
        public string Notes { get; set; }
        public string Reference { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
""")

with open(os.path.join(dtos_dir, "InventoryReservationDto.cs"), "w") as f:
    f.write("""using System;
namespace AliRoyalMarquee.Application.Inventory.DTOs
{
    public class InventoryReservationDto
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public decimal Quantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
""")

# Extend InventoryItemDto
inv_dto_path = os.path.join(dtos_dir, "InventoryItemDto.cs")
with open(inv_dto_path, "r") as f:
    code = f.read()
if "public decimal UnitPrice" not in code:
    code = code.replace("public string ItemType { get; set; }", 
                        "public string ItemType { get; set; }\n"
                        "        public decimal UnitPrice { get; set; }\n"
                        "        public string Location { get; set; }\n"
                        "        public System.Collections.Generic.List<InventoryMovementDto> Movements { get; set; } = new();\n"
                        "        public System.Collections.Generic.List<InventoryReservationDto> Reservations { get; set; } = new();")
    with open(inv_dto_path, "w") as f:
        f.write(code)

# Commands
cmds_dir = os.path.join(app_dir, "Commands")
with open(os.path.join(cmds_dir, "AddInventoryMovementCommand.cs"), "w") as f:
    f.write("""using System;
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
""")

with open(os.path.join(cmds_dir, "UpdateInventoryItemDetailsCommand.cs"), "w") as f:
    f.write("""using System;
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
""")

# GetInventoryItemsQuery mapping update
query_path = os.path.join(app_dir, "Queries", "GetInventoryItemsQuery.cs")
with open(query_path, "r") as f:
    code = f.read()
if "UnitPrice = x.UnitPrice," not in code:
    code = code.replace("Unit = x.Unit,", "Unit = x.Unit,\n                    UnitPrice = x.UnitPrice,\n                    Location = x.Location,")
    
    code = code.replace("using Microsoft.EntityFrameworkCore;", "using Microsoft.EntityFrameworkCore;\nusing System.Linq;")
    
    # We also need a GetById query to load movements and reservations
    with open(query_path, "w") as f:
        f.write(code)

with open(os.path.join(app_dir, "Queries", "GetInventoryItemByIdQuery.cs"), "w") as f:
    f.write("""using System;
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
""")

print("Application commands and queries generated.")
