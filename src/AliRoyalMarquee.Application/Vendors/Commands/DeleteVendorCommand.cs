using System;
using System.Threading;
using System.Threading.Tasks;
using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using MediatR;

namespace AliRoyalMarquee.Application.Vendors.Commands
{
    public class DeleteVendorCommand : IRequest
    {
        public Guid Id { get; set; }
    }

    public class DeleteVendorCommandHandler : IRequestHandler<DeleteVendorCommand>
    {
        private readonly IAppDbContext _context;

        public DeleteVendorCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteVendorCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Vendors.FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity == null)
            {
                throw new Exception($"Entity {nameof(Vendor)} ({request.Id}) was not found.");
            }

            _context.Vendors.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
