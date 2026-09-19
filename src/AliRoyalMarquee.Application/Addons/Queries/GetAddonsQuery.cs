using MediatR;
using AliRoyalMarquee.Domain.Entities;
using AliRoyalMarquee.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AliRoyalMarquee.Application.Addons.Queries;

public record GetAddonsQuery : IRequest<List<Addon>>;

public class GetAddonsQueryHandler : IRequestHandler<GetAddonsQuery, List<Addon>>
{
    private readonly IAppDbContext _context;
    public GetAddonsQueryHandler(IAppDbContext context) => _context = context;

    public async Task<List<Addon>> Handle(GetAddonsQuery request, CancellationToken cancellationToken)
    {
        return await _context.Addons.AsNoTracking().ToListAsync(cancellationToken);
    }
}
