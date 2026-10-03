using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Application.StaffMembers.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AliRoyalMarquee.Application.StaffMembers.Queries
{
    public class GetStaffQuery : IRequest<List<StaffDto>>
    {
    }

    public class GetStaffQueryHandler : IRequestHandler<GetStaffQuery, List<StaffDto>>
    {
        private readonly IAppDbContext _context;

        public GetStaffQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<StaffDto>> Handle(GetStaffQuery request, CancellationToken cancellationToken)
        {
            return await _context.StaffMembers
                .Select(s => new StaffDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Role = s.Role,
                    Phone = s.Phone,
                    Shift = s.Shift,
                    Status = s.Status,
                    Salary = s.Salary,
                    CNIC = s.CNIC,
                    CompensationType = s.CompensationType,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync(cancellationToken);
        }
    }
}
