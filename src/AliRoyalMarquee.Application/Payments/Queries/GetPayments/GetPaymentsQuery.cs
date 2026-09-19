using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Payments.Queries.GetPayments;

public record PaymentDto(
    Guid Id, 
    Guid BookingId, 
    Guid CustomerId, 
    string CustomerName, 
    decimal Amount, 
    string Method, 
    string DateStr, 
    string Status, 
    string? Reference
);

public record GetPaymentsQuery() : IRequest<List<PaymentDto>>;

public class GetPaymentsQueryHandler : IRequestHandler<GetPaymentsQuery, List<PaymentDto>>
{
    private readonly IAppDbContext _context;

    public GetPaymentsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PaymentDto>> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
    {
        var payments = await _context.Payments
            .Include(p => p.Booking)
            .ThenInclude(b => b.Customer)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);

        return payments.Select(p => new PaymentDto(
            p.Id,
            p.BookingId,
            p.Booking.CustomerId,
            p.Booking.Customer.Name,
            p.Amount,
            p.Method.ToString(),
            p.PaymentDate.ToString("yyyy-MM-dd"),
            p.Status.ToString(),
            p.ReferenceNumber
        )).ToList();
    }
}
