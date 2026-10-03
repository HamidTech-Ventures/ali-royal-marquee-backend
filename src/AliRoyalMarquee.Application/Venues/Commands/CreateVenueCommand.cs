using System;
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
