using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Customers.Queries.GetCustomerById;

public record CustomerDetailsDto(
    Guid Id, 
    string Name, 
    string Phone, 
    string? Email, 
    string Tier, 
    decimal TotalSpent,
    List<CustomerBookingDto> Bookings,
    List<CustomerEnquiryDto> Enquiries,
    List<CustomerPaymentDto> Payments
);

public record CustomerBookingDto(Guid Id, string Hall, string DateStr, string Status, int Guests, decimal TotalAmount);
public record CustomerEnquiryDto(Guid Id, string DateStr, string Status, int Guests);
public record CustomerPaymentDto(Guid Id, Guid BookingId, string DateStr, decimal Amount, string Method, string Reference);

public record GetCustomerByIdQuery(Guid Id) : IRequest<CustomerDetailsDto>;

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDetailsDto>
{
    private readonly IAppDbContext _context;

    public GetCustomerByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<CustomerDetailsDto> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _context.Customers
            .Include(c => c.Bookings)
            .ThenInclude(b => b.Payments)
            .Include(c => c.Bookings)
            .ThenInclude(b => b.Venue)
            .Include(c => c.Enquiries)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (customer == null) throw new Exception("Customer not found");

        var bookings = customer.Bookings.Select(b => new CustomerBookingDto(
            b.Id,
            b.Venue?.Name ?? "Unknown",
            b.BookingDate.ToString("yyyy-MM-dd"),
            b.Status.ToString(),
            b.GuestCount,
            b.TotalAmount
        )).ToList();

        var enquiries = customer.Enquiries.Select(e => new CustomerEnquiryDto(
            e.Id,
            e.PreferredDate.ToString("yyyy-MM-dd"),
            e.Status.ToString(),
            e.GuestCount
        )).ToList();

        var allPayments = customer.Bookings.SelectMany(b => b.Payments).Select(p => new CustomerPaymentDto(
            p.Id,
            p.BookingId,
            p.PaymentDate.ToString("yyyy-MM-dd"),
            p.Amount,
            p.Method.ToString(),
            p.ReferenceNumber
        )).ToList();

        return new CustomerDetailsDto(
            customer.Id,
            customer.Name,
            customer.Phone,
            customer.Email,
            customer.Tier.ToString(),
            customer.Bookings.SelectMany(b => b.Payments).Sum(p => p.Amount),
            bookings,
            enquiries,
            allPayments
        );
    }
}
