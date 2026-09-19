using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Users.Queries.GetStaff;

public record GetStaffQuery : IRequest<List<StaffDto>>;

public class StaffDto
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string RoleName { get; init; } = default!;
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
        return await _context.Users
            .AsNoTracking()
            .Include(u => u.Role)
            .Where(u => u.Status == UserStatus.Active)
            .OrderBy(u => u.FullName)
            .Select(u => new StaffDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                RoleName = u.Role.Name
            })
            .ToListAsync(cancellationToken);
    }
}
