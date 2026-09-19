using System;
using System.Threading;
using System.Threading.Tasks;
using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;

namespace AliRoyalMarquee.Application.StaffMembers.Commands
{
    public class UpdateStaffCommand : IRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string Phone { get; set; }
        public string Shift { get; set; }
        public string Status { get; set; }
        public decimal? Salary { get; set; }
    }

    public class UpdateStaffCommandHandler : IRequestHandler<UpdateStaffCommand>
    {
        private readonly IAppDbContext _context;

        public UpdateStaffCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateStaffCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.StaffMembers.FindAsync(new object[] { request.Id }, cancellationToken);

            if (entity == null)
            {
                throw new Exception($"StaffMember with ID {request.Id} not found.");
            }

            entity.Name = request.Name;
            entity.Role = request.Role;
            entity.Phone = request.Phone;
            entity.Shift = request.Shift;
            entity.Status = request.Status;
            entity.Salary = request.Salary;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
