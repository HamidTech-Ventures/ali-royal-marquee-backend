using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AliRoyalMarquee.Application.Bookings.Queries.CheckAvailability;

public record CheckAvailabilityQuery(Guid VenueId, DateTime StartTime, DateTime EndTime) : IRequest<bool>;

public class CheckAvailabilityValidator : AbstractValidator<CheckAvailabilityQuery>
{
    public CheckAvailabilityValidator()
    {
        RuleFor(v => v.VenueId).NotEmpty();
        RuleFor(v => v.StartTime).LessThan(v => v.EndTime);
    }
}

public class CheckAvailabilityQueryHandler : IRequestHandler<CheckAvailabilityQuery, bool>
{
    private readonly IAppDbContext _context;

    public CheckAvailabilityQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(CheckAvailabilityQuery request, CancellationToken cancellationToken)
    {
        // Returns true if available (no overlaps)
        var overlapExists = await _context.Bookings
            .AnyAsync(b => b.VenueId == request.VenueId 
                        && b.StartTime < request.EndTime 
                        && b.EndTime > request.StartTime 
                        && b.Status != Domain.Enums.BookingStatus.Cancelled, 
                        cancellationToken);

        return !overlapExists;
    }
}
