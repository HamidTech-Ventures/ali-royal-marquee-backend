using System;
using System.Threading;
using System.Threading.Tasks;
using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using MediatR;

namespace AliRoyalMarquee.Application.Packages.Commands
{
    public class CreatePackageCommand : IRequest<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int MinGuests { get; set; }
        
        public string? InternalNotes { get; set; }
        public string? InclusionsJson { get; set; }
    }

    public class CreatePackageCommandHandler : IRequestHandler<CreatePackageCommand, Guid>
    {
        private readonly IAppDbContext _context;

        public CreatePackageCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreatePackageCommand request, CancellationToken cancellationToken)
        {
            var package = new Package
            {
                Name = request.Name,
                Type = request.Type,
                Price = request.Price,
                MinGuests = request.MinGuests,
                
                InternalNotes = request.InternalNotes,
                InclusionsJson = request.InclusionsJson,
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Packages.Add(package);
            await _context.SaveChangesAsync(cancellationToken);

            return package.Id;
        }
    }
}
