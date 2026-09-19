using System;
using System.Threading;
using System.Threading.Tasks;
using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using MediatR;

namespace AliRoyalMarquee.Application.Vendors.Commands
{
    public class CreateVendorCommand : IRequest<Guid>
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string ContactName { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
    }

    public class CreateVendorCommandHandler : IRequestHandler<CreateVendorCommand, Guid>
    {
        private readonly IAppDbContext _context;

        public CreateVendorCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateVendorCommand request, CancellationToken cancellationToken)
        {
            var entity = new Vendor
            {
                Name = request.Name,
                Category = request.Category,
                ContactName = request.ContactName,
                Phone = request.Phone,
                Status = request.Status
            };

            _context.Vendors.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}
