using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Application.Vendors.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AliRoyalMarquee.Application.Vendors.Queries
{
    public class GetVendorsQuery : IRequest<List<VendorDto>>
    {
    }

    public class GetVendorsQueryHandler : IRequestHandler<GetVendorsQuery, List<VendorDto>>
    {
        private readonly IAppDbContext _context;

        public GetVendorsQueryHandler(IAppDbContext context)
        {
            _context = context;
        }

        public async Task<List<VendorDto>> Handle(GetVendorsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Vendors
                .AsNoTracking()
                .Select(x => new VendorDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Category = x.Category,
                    ContactName = x.ContactName,
                    Phone = x.Phone,
                    Status = x.Status
                })
                .ToListAsync(cancellationToken);
        }
    }
}
