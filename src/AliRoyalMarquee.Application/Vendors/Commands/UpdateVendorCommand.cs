using System;
using System.Threading;
using System.Threading.Tasks;
using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using MediatR;

namespace AliRoyalMarquee.Application.Vendors.Commands
{
    public class UpdateVendorCommand : IRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string ContactName { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
    }

    public class UpdateVendorCommandHandler : IRequestHandler<UpdateVendorCommand>
    {
        private readonly IAppDbContext _context;

        public UpdateVendorCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateVendorCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Vendors.FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity == null)
            {
                throw new Exception($"Entity {nameof(Vendor)} ({request.Id}) was not found.");
            }

            entity.Name = request.Name;
            entity.Category = request.Category;
            entity.ContactName = request.ContactName;
            entity.Phone = request.Phone;
            entity.Status = request.Status;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
