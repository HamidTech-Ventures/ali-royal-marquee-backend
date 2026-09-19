using MediatR;
using AliRoyalMarquee.Domain.Entities;
using AliRoyalMarquee.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AliRoyalMarquee.Application.MenuItems.Queries;

public record GetMenuItemsQuery : IRequest<List<MenuItem>>;

public class GetMenuItemsQueryHandler : IRequestHandler<GetMenuItemsQuery, List<MenuItem>>
{
    private readonly IAppDbContext _context;
    public GetMenuItemsQueryHandler(IAppDbContext context) => _context = context;

    public async Task<List<MenuItem>> Handle(GetMenuItemsQuery request, CancellationToken cancellationToken)
    {
        return await _context.MenuItems.AsNoTracking().ToListAsync(cancellationToken);
    }
}
