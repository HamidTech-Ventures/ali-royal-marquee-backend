import os

backend_src = r"c:\My working\HamidTech_Ventures\Clients\marquee-management-system\backend\src"
app_dir = os.path.join(backend_src, "AliRoyalMarquee.Application")
api_dir = os.path.join(backend_src, "AliRoyalMarquee.API", "Controllers")
domain_dir = os.path.join(backend_src, "AliRoyalMarquee.Domain", "Entities")
frontend_src = r"c:\My working\HamidTech_Ventures\Clients\marquee-management-system\frontend\src"

# 1. Update Venue Entity
venue_path = os.path.join(domain_dir, "Venue.cs")
with open(venue_path, "r") as f:
    venue_code = f.read()

if "public void UpdateDetails" not in venue_code:
    update_method = """
    public void UpdateDetails(string name, int capacity, string? description)
    {
        Name = name;
        Capacity = capacity;
        Description = description;
    }
"""
    venue_code = venue_code.replace("public void UpdateStatus(bool isActive)", update_method + "    public void UpdateStatus(bool isActive)")
    with open(venue_path, "w") as f:
        f.write(venue_code)

# 2. CreateVenueCommand
create_cmd_path = os.path.join(app_dir, "Venues", "Commands", "CreateVenueCommand.cs")
os.makedirs(os.path.dirname(create_cmd_path), exist_ok=True)
with open(create_cmd_path, "w") as f:
    f.write("""using System;
using System.Threading;
using System.Threading.Tasks;
using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using MediatR;

namespace AliRoyalMarquee.Application.Venues.Commands
{
    public class CreateVenueCommand : IRequest<Guid>
    {
        public string Name { get; set; } = default!;
        public int Capacity { get; set; }
        public string? Description { get; set; }
    }

    public class CreateVenueCommandHandler : IRequestHandler<CreateVenueCommand, Guid>
    {
        private readonly IAppDbContext _context;
        public CreateVenueCommandHandler(IAppDbContext context) { _context = context; }

        public async Task<Guid> Handle(CreateVenueCommand request, CancellationToken cancellationToken)
        {
            var venue = new Venue(request.Name, request.Capacity, request.Description);
            _context.Venues.Add(venue);
            await _context.SaveChangesAsync(cancellationToken);
            return venue.Id;
        }
    }
}
""")

# 3. UpdateVenueCommand
update_cmd_path = os.path.join(app_dir, "Venues", "Commands", "UpdateVenueCommand.cs")
with open(update_cmd_path, "w") as f:
    f.write("""using System;
using System.Threading;
using System.Threading.Tasks;
using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;

namespace AliRoyalMarquee.Application.Venues.Commands
{
    public class UpdateVenueCommand : IRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public int Capacity { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateVenueCommandHandler : IRequestHandler<UpdateVenueCommand>
    {
        private readonly IAppDbContext _context;
        public UpdateVenueCommandHandler(IAppDbContext context) { _context = context; }

        public async Task Handle(UpdateVenueCommand request, CancellationToken cancellationToken)
        {
            var venue = await _context.Venues.FindAsync(new object[] { request.Id }, cancellationToken);
            if (venue == null) throw new Exception("Venue not found");

            venue.UpdateDetails(request.Name, request.Capacity, request.Description);
            venue.UpdateStatus(request.IsActive);
            
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
""")

# 4. Update VenuesController
ctrl_path = os.path.join(api_dir, "VenuesController.cs")
with open(ctrl_path, "r") as f:
    ctrl_code = f.read()

if "CreateVenue" not in ctrl_code:
    endpoints = """
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateVenue(AliRoyalMarquee.Application.Venues.Commands.CreateVenueCommand command)
    {
        return await _mediator.Send(command);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateVenue(Guid id, AliRoyalMarquee.Application.Venues.Commands.UpdateVenueCommand command)
    {
        if (id != command.Id) return BadRequest();
        await _mediator.Send(command);
        return NoContent();
    }
"""
    ctrl_code = ctrl_code.replace("return Ok(result);\n    }", "return Ok(result);\n    }\n" + endpoints)
    with open(ctrl_path, "w") as f:
        f.write(ctrl_code)

# 5. Update referenceService.ts
ref_svc_path = os.path.join(frontend_src, "services", "referenceService.ts")
with open(ref_svc_path, "r") as f:
    ref_code = f.read()

if "addVenue:" not in ref_code:
    add_methods = """
  addVenue: async (data: any): Promise<string> => {
    const response = await api.post('/venues', data);
    return response.data;
  },
  updateVenue: async (id: string, data: any): Promise<void> => {
    await api.put(`/venues/${id}`, data);
  },
"""
    ref_code = ref_code.replace("getVenues: async", add_methods + "  getVenues: async")
    with open(ref_svc_path, "w") as f:
        f.write(ref_code)

# 6. Update Settings.tsx
settings_path = os.path.join(frontend_src, "features", "settings", "Settings.tsx")
with open(settings_path, "r") as f:
    settings_code = f.read()

if "const [venues, setVenues]" not in settings_code:
    # imports
    settings_code = settings_code.replace("import { settingsApi, type SystemSetting } from '../../services/settingsApi';", 
                                          "import { settingsApi, type SystemSetting } from '../../services/settingsApi';\nimport { referenceService } from '../../services/referenceService';")
    # states
    states = """
  const [venues, setVenues] = useState<any[]>([]);
  const [showVenueModal, setShowVenueModal] = useState(false);
  const [editingVenue, setEditingVenue] = useState<any>(null);
  const [venueForm, setVenueForm] = useState({ name: '', capacity: '', description: '', isActive: true });
"""
    settings_code = settings_code.replace("const [isLoading, setIsLoading] = useState(true);", 
                                          "const [isLoading, setIsLoading] = useState(true);" + states)
    
    # fetch
    fetch_insert = """
      const vData = await referenceService.getVenues();
      setVenues(vData);
"""
    settings_code = settings_code.replace("const data = await settingsApi.getSettings();", 
                                          "const data = await settingsApi.getSettings();\n" + fetch_insert)
    
    # handlers
    handlers = """
  const handleSaveVenue = async () => {
    try {
      if (editingVenue) {
        await referenceService.updateVenue(editingVenue.id, { id: editingVenue.id, ...venueForm, capacity: Number(venueForm.capacity) });
      } else {
        await referenceService.addVenue({ ...venueForm, capacity: Number(venueForm.capacity) });
      }
      const vData = await referenceService.getVenues();
      setVenues(vData);
      setShowVenueModal(false);
      setEditingVenue(null);
      setVenueForm({ name: '', capacity: '', description: '', isActive: true });
      success(editingVenue ? 'Venue updated!' : 'Venue added!');
    } catch(e) {
      showError('Failed to save venue');
    }
  };
"""
    settings_code = settings_code.replace("const handleSave = async () => {", handlers + "  const handleSave = async () => {")

    # Sidebar link
    sidebar_old = """                  <span className="material-symbols-outlined text-[16px] text-on-surface-variant opacity-80 group-hover:text-primary transition-colors">chevron_right</span>
                </a>
              </nav>
            </div>"""
    sidebar_new = """                  <span className="material-symbols-outlined text-[16px] text-on-surface-variant opacity-80 group-hover:text-primary transition-colors">chevron_right</span>
                </a>
                <a className="group flex items-center justify-between px-3 py-2 rounded text-on-surface-variant hover:bg-surface-container-low hover:text-primary transition-all" href="#venues-management">
                  <div className="flex items-center gap-2.5 min-w-0">
                    <span className="material-symbols-outlined text-[18px]">meeting_room</span>
                    <span className="font-title-sm text-title-sm truncate">Venues Management</span>
                  </div>
                  <span className="material-symbols-outlined text-[16px] text-on-surface-variant opacity-80 group-hover:text-primary transition-colors">chevron_right</span>
                </a>
              </nav>
            </div>"""
    settings_code = settings_code.replace(sidebar_old, sidebar_new)

    # UI Content
    ui_insert = """
          {/* VENUES MANAGEMENT */}
          <div id="venues-management" className="bg-surface-container-lowest rounded-xl shadow-sm border border-outline-variant/30 overflow-hidden mb-8 scroll-mt-24">
            <div className="p-6 border-b border-outline-variant/30 flex justify-between items-center bg-surface">
              <div>
                <h2 className="font-title-lg text-title-lg text-on-surface flex items-center gap-2">
                  <span className="material-symbols-outlined text-primary">meeting_room</span>
                  Venues Management
                </h2>
                <p className="text-body-sm text-on-surface-variant mt-1">Manage halls, capacities, and active status for enquiries.</p>
              </div>
              <Button variant="primary" icon="add" onClick={() => { setEditingVenue(null); setVenueForm({ name: '', capacity: '', description: '', isActive: true }); setShowVenueModal(true); }}>Add Venue</Button>
            </div>
            <div className="p-6">
              <table className="w-full text-left border-collapse">
                <thead className="bg-surface-variant/30 text-on-surface-variant text-sm">
                  <tr>
                    <th className="p-3">Name</th>
                    <th className="p-3">Capacity</th>
                    <th className="p-3">Description</th>
                    <th className="p-3">Status</th>
                    <th className="p-3">Actions</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-outline-variant/20 text-sm">
                  {venues.map((v: any) => (
                    <tr key={v.id} className="hover:bg-surface-variant/10">
                      <td className="p-3 font-semibold text-primary">{v.name}</td>
                      <td className="p-3">{v.capacity} Guests</td>
                      <td className="p-3">{v.description || '-'}</td>
                      <td className="p-3">
                        <span className={`px-2 py-1 rounded text-xs font-semibold ${v.isActive ? 'bg-success/20 text-success' : 'bg-error/20 text-error'}`}>
                          {v.isActive ? 'Active' : 'Inactive'}
                        </span>
                      </td>
                      <td className="p-3">
                        <Button variant="text" onClick={() => { setEditingVenue(v); setVenueForm({ name: v.name, capacity: v.capacity, description: v.description || '', isActive: v.isActive }); setShowVenueModal(true); }}>Edit</Button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
              
              {showVenueModal && (
                <div className="fixed inset-0 z-50 bg-black/50 flex items-center justify-center p-4">
                  <div className="bg-surface w-full max-w-md rounded-2xl p-6">
                    <h3 className="font-title-lg mb-4">{editingVenue ? 'Edit Venue' : 'Add New Venue'}</h3>
                    <div className="space-y-4">
                      <div>
                        <label className="block text-sm font-medium mb-1">Venue Name</label>
                        <input type="text" className="w-full rounded-lg border border-outline-variant p-2" value={venueForm.name} onChange={e => setVenueForm({...venueForm, name: e.target.value})} placeholder="e.g. Hall A"/>
                      </div>
                      <div>
                        <label className="block text-sm font-medium mb-1">Max Capacity</label>
                        <input type="number" className="w-full rounded-lg border border-outline-variant p-2" value={venueForm.capacity} onChange={e => setVenueForm({...venueForm, capacity: e.target.value})} placeholder="e.g. 500"/>
                      </div>
                      <div>
                        <label className="block text-sm font-medium mb-1">Description</label>
                        <input type="text" className="w-full rounded-lg border border-outline-variant p-2" value={venueForm.description} onChange={e => setVenueForm({...venueForm, description: e.target.value})}/>
                      </div>
                      <div className="flex items-center gap-2">
                        <input type="checkbox" checked={venueForm.isActive} onChange={e => setVenueForm({...venueForm, isActive: e.target.checked})}/>
                        <label className="text-sm font-medium">Is Active</label>
                      </div>
                      <div className="flex justify-end gap-2 mt-6">
                        <Button variant="text" onClick={() => setShowVenueModal(false)}>Cancel</Button>
                        <Button variant="primary" onClick={handleSaveVenue}>Save Venue</Button>
                      </div>
                    </div>
                  </div>
                </div>
              )}
            </div>
          </div>
"""
    # Find a good place to insert this - maybe right before the Tax configuration block or just at the bottom of the right pane.
    tax_section = '{/* FINANCIAL & TAX CONFIGURATION */}'
    settings_code = settings_code.replace(tax_section, ui_insert + "\n          " + tax_section)
    
    with open(settings_path, "w") as f:
        f.write(settings_code)

print("Settings Venue UI and Backend successfully generated!")
