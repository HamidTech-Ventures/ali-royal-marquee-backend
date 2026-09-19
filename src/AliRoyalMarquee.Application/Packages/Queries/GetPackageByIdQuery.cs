using MediatR;
using AliRoyalMarquee.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using AliRoyalMarquee.Domain.Entities;

namespace AliRoyalMarquee.Application.Packages.Queries;

public record GetPackageByIdQuery(Guid Id) : IRequest<Package?>;

public class GetPackageByIdQueryHandler : IRequestHandler<GetPackageByIdQuery, Package?>
{
    private readonly IAppDbContext _context;

    public GetPackageByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Package?> Handle(GetPackageByIdQuery request, CancellationToken cancellationToken)
    {
        var package = await _context.Packages
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        return package;
    }
}
