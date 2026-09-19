using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Enquiries.Commands.UpdateEnquiry;

public record UpdateEnquiryCommand(
    Guid Id,
    string EventName,
    string? EventType,
    DateOnly PreferredDate,
    DateOnly? AlternativeDate,
    TimeOnly? PreferredStartTime,
    TimeOnly? PreferredEndTime,
    int GuestCount,
    Guid? PreferredVenueId,
    decimal? Budget,
    EnquirySource Source,
    EnquiryPriority Priority,
    Guid? AssignedToId,
    string? Notes,
    decimal? EstimatedValue) : IRequest;

public class UpdateEnquiryValidator : AbstractValidator<UpdateEnquiryCommand>
{
    public UpdateEnquiryValidator()
    {
        RuleFor(v => v.Id).NotEmpty();
        RuleFor(v => v.EventName).NotEmpty().MaximumLength(100);
        RuleFor(v => v.GuestCount).GreaterThan(0);
    }
}

public class UpdateEnquiryCommandHandler : IRequestHandler<UpdateEnquiryCommand>
{
    private readonly IAppDbContext _context;

    public UpdateEnquiryCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateEnquiryCommand request, CancellationToken cancellationToken)
    {
        var enquiry = await _context.Enquiries
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (enquiry == null)
            throw new Exception("Enquiry not found"); // Custom NotFoundException would be better but keeping it simple

        enquiry.UpdateDetails(
            request.EventName,
            request.EventType,
            request.PreferredDate,
            request.AlternativeDate,
            request.PreferredStartTime,
            request.PreferredEndTime,
            request.GuestCount,
            request.PreferredVenueId,
            request.Budget,
            request.Source,
            request.Priority,
            request.AssignedToId,
            request.Notes,
            request.EstimatedValue
        );

        await _context.SaveChangesAsync(cancellationToken);
    }
}
