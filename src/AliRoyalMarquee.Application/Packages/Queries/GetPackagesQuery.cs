using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AliRoyalMarquee.Application.Packages.Queries
{
    public class GetPackagesQuery : IRequest<List<Package>>
    {
    }

    public class GetPackagesQueryHandler : IRequestHandler<GetPackagesQuery, List<Package>>
    {
        private readonly IAppDbContext _context;

        public GetPackagesQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Package>> Handle(GetPackagesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Packages
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
