using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AliRoyalMarquee.Application.Auth.Queries.Me;

public class MeQueryHandler : IRequestHandler<MeQuery, MeResult>
{
    private readonly IAppDbContext _context;

    public MeQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<MeResult> Handle(MeQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found.");
        }

        var permissions = user.Role.RolePermissions.Select(rp => rp.Permission.Name).ToList();

        return new MeResult(
            user.Id,
            user.FullName,
            user.Email,
            user.Role.Name,
            user.Status.ToString(),
            permissions
        );
    }
}
