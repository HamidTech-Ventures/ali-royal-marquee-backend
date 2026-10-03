using System;
using System.Threading;
using System.Threading.Tasks;
using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using MediatR;

namespace AliRoyalMarquee.Application.StaffMembers.Commands
{
    public class CreateStaffCommand : IRequest<Guid>
    {
        public string Name { get; set; }
        public string Role { get; set; }
        public string Phone { get; set; }
        public string Shift { get; set; }
        public string Status { get; set; }
        public decimal? Salary { get; set; }
        public string CNIC { get; set; }
        public string CompensationType { get; set; }
    }

    public class CreateStaffCommandHandler : IRequestHandler<CreateStaffCommand, Guid>
    {
        private readonly IAppDbContext _context;

        public CreateStaffCommandHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateStaffCommand request, CancellationToken cancellationToken)
        {
            var entity = new StaffMember
            {
                Name = request.Name,
                Role = request.Role,
                Phone = request.Phone,
                Shift = request.Shift,
                Status = request.Status,
                Salary = request.Salary,
                CNIC = request.CNIC,
                CompensationType = request.CompensationType
            };

            _context.StaffMembers.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}
