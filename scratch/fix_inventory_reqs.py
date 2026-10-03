import os

# 1. Update Inventory.tsx for filtering
frontend_src = r"c:\My working\HamidTech_Ventures\Clients\marquee-management-system\frontend\src"
inv_path = os.path.join(frontend_src, "features", "inventory", "Inventory.tsx")

with open(inv_path, "r") as f:
    inv_code = f.read()

filter_old = "result = result.filter(i => i.itemType === activeTab);"
filter_new = """    result = result.filter(i => {
      if (activeTab === 'Fixed Asset') return i.itemType?.includes('Fixed') || i.itemType === 'FixedAsset';
      if (activeTab === 'Consumable') return i.itemType?.includes('Consumable');
      return true;
    });"""
inv_code = inv_code.replace(filter_old, filter_new)

with open(inv_path, "w") as f:
    f.write(inv_code)


# 2. Update InventoryDetails.tsx for real Venues
inv_det_path = os.path.join(frontend_src, "features", "inventory", "InventoryDetails.tsx")
with open(inv_det_path, "r") as f:
    det_code = f.read()

# Add referenceService
if "import { referenceService }" not in det_code:
    det_code = det_code.replace("import { eventsService } from '../../services/eventsService';",
                                "import { eventsService } from '../../services/eventsService';\nimport { referenceService } from '../../services/referenceService';")

# Add state
if "const [venues, setVenues] = useState<any[]>([]);" not in det_code:
    det_code = det_code.replace("const [events, setEvents] = useState<Event[]>([]);",
                                "const [events, setEvents] = useState<Event[]>([]);\n  const [venues, setVenues] = useState<any[]>([]);")
    
    # Update fetch
    fetch_old = """    const fetchEvents = async () => {
      try {
        const evData = await eventsService.getEvents();
        setEvents(evData);
      } catch(e) {}
    };"""
    fetch_new = """    const fetchEvents = async () => {
      try {
        const evData = await eventsService.getEvents();
        setEvents(evData);
        const vData = await referenceService.getVenues();
        setVenues(vData);
      } catch(e) {}
    };"""
    det_code = det_code.replace(fetch_old, fetch_new)

# Update the select dropdown in Movement Form
select_old = """                    <select className="w-full rounded-lg border border-outline-variant p-2" value={movementForm.location} onChange={e => setMovementForm({...movementForm, location: e.target.value})}>
                      <option value="">Select Location...</option>
                      <option value="Main Store">Main Store</option>
                      <option value="Hall A (Royal)">Hall A (Royal)</option>
                      <option value="Hall B (Crown)">Hall B (Crown)</option>
                      <option value="Kitchen">Kitchen</option>
                    </select>"""
select_new = """                    <select className="w-full rounded-lg border border-outline-variant p-2" value={movementForm.location} onChange={e => setMovementForm({...movementForm, location: e.target.value})}>
                      <option value="">Select Location...</option>
                      <option value="Main Store">Main Store</option>
                      <option value="Kitchen">Kitchen</option>
                      {venues.map((v: any) => (
                        <option key={v.id} value={v.name}>{v.name}</option>
                      ))}
                    </select>"""
det_code = det_code.replace(select_old, select_new)

with open(inv_det_path, "w") as f:
    f.write(det_code)


# 3. Update AddInventoryReservationCommand.cs
cmd_path = r"c:\My working\HamidTech_Ventures\Clients\marquee-management-system\backend\src\AliRoyalMarquee.Application\Inventory\Commands\AddInventoryReservationCommand.cs"
with open(cmd_path, "r") as f:
    cmd_code = f.read()

handler_old = """            var reservation = new InventoryReservation
            {
                InventoryItemId = request.InventoryItemId,
                EventId = request.EventId,
                Quantity = request.Quantity,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = request.Status
            };"""
handler_new = """            var reservation = new InventoryReservation
            {
                InventoryItemId = request.InventoryItemId,
                EventId = request.EventId,
                Quantity = request.Quantity,
                StartDate = request.StartDate.ToUniversalTime(),
                EndDate = request.EndDate.ToUniversalTime(),
                Status = request.Status
            };"""
cmd_code = cmd_code.replace(handler_old, handler_new)

with open(cmd_path, "w") as f:
    f.write(cmd_code)

print("Fixes applied successfully.")
