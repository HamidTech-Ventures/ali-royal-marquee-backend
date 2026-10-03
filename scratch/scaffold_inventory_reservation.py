import os

backend_src = r"c:\My working\HamidTech_Ventures\Clients\marquee-management-system\backend\src"
app_dir = os.path.join(backend_src, "AliRoyalMarquee.Application")
api_dir = os.path.join(backend_src, "AliRoyalMarquee.API", "Controllers")
frontend_src = r"c:\My working\HamidTech_Ventures\Clients\marquee-management-system\frontend\src"

# 1. Create AddInventoryReservationCommand
cmd_path = os.path.join(app_dir, "Inventory", "Commands", "AddInventoryReservationCommand.cs")
with open(cmd_path, "w") as f:
    f.write("""using System;
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
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = request.Status
            };

            _context.InventoryReservations.Add(reservation);
            await _context.SaveChangesAsync(cancellationToken);
            return reservation.Id;
        }
    }
}
""")

# 2. Update Controller
ctrl_path = os.path.join(api_dir, "InventoryController.cs")
with open(ctrl_path, "r") as f:
    code = f.read()
if "AddReservation" not in code:
    code = code.replace("public async Task<ActionResult<Guid>> AddMovement(AddInventoryMovementCommand command)", 
                        "[HttpPost(\"reservations\")]\n"
                        "        public async Task<ActionResult<Guid>> AddReservation(AddInventoryReservationCommand command)\n"
                        "        {\n"
                        "            return await _mediator.Send(command);\n"
                        "        }\n\n"
                        "        [HttpPost(\"movements\")]\n"
                        "        public async Task<ActionResult<Guid>> AddMovement(AddInventoryMovementCommand command)")
    with open(ctrl_path, "w") as f:
        f.write(code)

# 3. Update Frontend Service
service_path = os.path.join(frontend_src, "services", "inventoryService.ts")
with open(service_path, "r") as f:
    code = f.read()
if "addInventoryReservation" not in code:
    code = code.replace("addInventoryMovement:", 
                        "addInventoryReservation: async (data: any): Promise<string> => {\n"
                        "    const response = await api.post('/inventory/reservations', data);\n"
                        "    return response.data;\n"
                        "  },\n\n  addInventoryMovement:")
    with open(service_path, "w") as f:
        f.write(code)

# 4. Update Frontend UI
ui_path = os.path.join(frontend_src, "features", "inventory", "InventoryDetails.tsx")
with open(ui_path, "r") as f:
    code = f.read()

# Add state
if "resForm" not in code:
    state_insert = """
  const [resForm, setResForm] = useState({ eventId: '', quantity: '', startDate: '', endDate: '', status: 'Active' });
  
  const handleRecordReservation = async () => {
    try {
      await inventoryService.addInventoryReservation({
        inventoryItemId: item.id,
        eventId: resForm.eventId || '00000000-0000-0000-0000-000000000000', // Mock event ID if empty
        quantity: Number(resForm.quantity),
        startDate: new Date(resForm.startDate).toISOString(),
        endDate: new Date(resForm.endDate).toISOString(),
        status: resForm.status
      });
      // Refresh
      const data = await inventoryService.getInventoryItemById(item.id);
      setItem(data);
      setShowResForm(false);
      setResForm({ eventId: '', quantity: '', startDate: '', endDate: '', status: 'Active' });
    } catch(e) {
      console.error(e);
      alert('Failed to record reservation');
    }
  };
"""
    code = code.replace("const [showResForm, setShowResForm] = useState(false);", 
                        "const [showResForm, setShowResForm] = useState(false);\n" + state_insert)

# Replace the dummy placeholder with a real form
res_old = """            {showResForm && (
              <div className="bg-surface rounded-xl border border-outline-variant/40 p-5 mb-4 flex items-center justify-center text-on-surface-variant">
                 Backend API for reservations is not yet linked. Forms will be active soon!
              </div>
            )}"""
res_new = """            {showResForm && (
              <div className="bg-surface rounded-xl border border-outline-variant/40 p-5 mb-4 grid grid-cols-2 md:grid-cols-6 gap-4 items-end">
                <div>
                  <label className="block text-sm font-medium mb-1">Event/Booking ID</label>
                  <input type="text" className="w-full rounded-lg border border-outline-variant p-2" value={resForm.eventId} onChange={e => setResForm({...resForm, eventId: e.target.value})} placeholder="Event GUID..."/>
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1">Quantity</label>
                  <input type="number" className="w-full rounded-lg border border-outline-variant p-2" value={resForm.quantity} onChange={e => setResForm({...resForm, quantity: e.target.value})} placeholder="Qty"/>
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1">Start Date</label>
                  <input type="date" className="w-full rounded-lg border border-outline-variant p-2" value={resForm.startDate} onChange={e => setResForm({...resForm, startDate: e.target.value})}/>
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1">End Date</label>
                  <input type="date" className="w-full rounded-lg border border-outline-variant p-2" value={resForm.endDate} onChange={e => setResForm({...resForm, endDate: e.target.value})}/>
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1">Status</label>
                  <select className="w-full rounded-lg border border-outline-variant p-2" value={resForm.status} onChange={e => setResForm({...resForm, status: e.target.value})}>
                    <option value="Active">Active</option>
                    <option value="Completed">Completed</option>
                    <option value="Cancelled">Cancelled</option>
                  </select>
                </div>
                <div>
                  <Button variant="primary" className="w-full py-2" onClick={handleRecordReservation}>Submit</Button>
                </div>
              </div>
            )}"""
code = code.replace(res_old, res_new)

with open(ui_path, "w") as f:
    f.write(code)

print("Reservation implementation completed.")
