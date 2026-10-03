import os

backend_src = r"c:\My working\HamidTech_Ventures\Clients\marquee-management-system\backend\src"
domain_dir = os.path.join(backend_src, "AliRoyalMarquee.Domain", "Entities")
app_dir = os.path.join(backend_src, "AliRoyalMarquee.Application")
api_dir = os.path.join(backend_src, "AliRoyalMarquee.API", "Controllers")

# 1. Update InventoryItem
inv_item_path = os.path.join(domain_dir, "InventoryItem.cs")
with open(inv_item_path, "r") as f:
    code = f.read()

if "public decimal UnitPrice" not in code:
    code = code.replace("public string ItemType { get; set; } = \"Fixed Asset\"; // Fixed Asset or Consumable", 
                        "public string ItemType { get; set; } = \"Fixed Asset\";\n"
                        "        public decimal UnitPrice { get; set; }\n"
                        "        public string Location { get; set; }\n"
                        "        public ICollection<InventoryMovement> Movements { get; set; } = new List<InventoryMovement>();\n"
                        "        public ICollection<InventoryReservation> Reservations { get; set; } = new List<InventoryReservation>();")
    with open(inv_item_path, "w") as f:
        f.write(code)

# 2. Create InventoryMovement Entity
movement_code = """using System;
using AliRoyalMarquee.Domain.Common;

namespace AliRoyalMarquee.Domain.Entities
{
    public class InventoryMovement : BaseEntity
    {
        public Guid InventoryItemId { get; set; }
        public InventoryItem InventoryItem { get; set; }
        public string Type { get; set; } // IN, OUT, RELOCATE
        public decimal Quantity { get; set; }
        public string Notes { get; set; }
        public string Reference { get; set; } // e.g. PO-123 or Event-456
    }
}
"""
with open(os.path.join(domain_dir, "InventoryMovement.cs"), "w") as f:
    f.write(movement_code)

# 3. Create InventoryReservation Entity
reservation_code = """using System;
using AliRoyalMarquee.Domain.Common;

namespace AliRoyalMarquee.Domain.Entities
{
    public class InventoryReservation : BaseEntity
    {
        public Guid InventoryItemId { get; set; }
        public InventoryItem InventoryItem { get; set; }
        public Guid EventId { get; set; }
        public Event Event { get; set; }
        public decimal Quantity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } // Active, Completed, Cancelled
    }
}
"""
with open(os.path.join(domain_dir, "InventoryReservation.cs"), "w") as f:
    f.write(reservation_code)

# 4. Update DbContext
context_path = os.path.join(app_dir, "Common", "Interfaces", "IAppDbContext.cs")
with open(context_path, "r") as f:
    code = f.read()
if "DbSet<InventoryMovement>" not in code:
    code = code.replace("DbSet<InventoryItem> InventoryItems { get; }", 
                        "DbSet<InventoryItem> InventoryItems { get; }\n"
                        "        DbSet<InventoryMovement> InventoryMovements { get; }\n"
                        "        DbSet<InventoryReservation> InventoryReservations { get; }")
    with open(context_path, "w") as f:
        f.write(code)

infra_context = os.path.join(backend_src, "AliRoyalMarquee.Infrastructure", "Persistence", "AppDbContext.cs")
with open(infra_context, "r") as f:
    code = f.read()
if "DbSet<InventoryMovement>" not in code:
    code = code.replace("public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();", 
                        "public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();\n"
                        "        public DbSet<InventoryMovement> InventoryMovements => Set<InventoryMovement>();\n"
                        "        public DbSet<InventoryReservation> InventoryReservations => Set<InventoryReservation>();")
    with open(infra_context, "w") as f:
        f.write(code)

print("Entities generated and DbContext updated.")
