using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Customers.Queries.GetCustomers;

public record CustomerDto(Guid Id, string Name, string Phone, string? Email, string Tier, decimal TotalSpent);

public record GetCustomersQuery() : IRequest<List<CustomerDto>>;

public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, List<CustomerDto>>
{
    private readonly IAppDbContext _context;

    public GetCustomersQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await _context.Customers
            .Include(c => c.Bookings)
            .ThenInclude(b => b.Payments)
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

        return customers.Select(c => new CustomerDto(
            c.Id,
            c.Name,
            c.Phone,
            c.Email,
            c.Tier.ToString(),
            c.Bookings.SelectMany(b => b.Payments).Sum(p => p.Amount)
        )).ToList();
    }
}
