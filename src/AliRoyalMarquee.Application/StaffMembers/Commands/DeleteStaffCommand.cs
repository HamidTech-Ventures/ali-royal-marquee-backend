using System;
using System.Threading;
using System.Threading.Tasks;
using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;

namespace AliRoyalMarquee.Application.StaffMembers.Commands
{
    public class DeleteStaffCommand : IRequest
    {
        public Guid Id { get; set; }
    }

    public class DeleteStaffCommandHandler : IRequestHandler<DeleteStaffCommand>
    {
        private readonly IAppDbContext _context;

        public DeleteStaffCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteStaffCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.StaffMembers.FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity == null)
            {
                throw new Exception($"StaffMember with ID {request.Id} not found.");
            }

            _context.StaffMembers.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
