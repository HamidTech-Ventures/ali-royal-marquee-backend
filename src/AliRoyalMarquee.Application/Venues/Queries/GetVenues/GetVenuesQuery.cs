using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Venues.Queries.GetVenues;

public record GetVenuesQuery : IRequest<List<VenueDto>>;

public class VenueDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public int Capacity { get; init; }
    public string? Description { get; init; }
}

public class GetVenuesQueryHandler : IRequestHandler<GetVenuesQuery, List<VenueDto>>
{
    private readonly IAppDbContext _context;

    public GetVenuesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<VenueDto>> Handle(GetVenuesQuery request, CancellationToken cancellationToken)
    {
        return await _context.Venues
            .AsNoTracking()
            .OrderBy(v => v.Name)
            .Select(v => new VenueDto
            {
                Id = v.Id,
                Name = v.Name,
                Capacity = v.Capacity,
                Description = v.Description
            })
            .ToListAsync(cancellationToken);
    }
}
