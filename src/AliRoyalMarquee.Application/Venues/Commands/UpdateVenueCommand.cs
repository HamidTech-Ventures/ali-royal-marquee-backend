using System;
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
