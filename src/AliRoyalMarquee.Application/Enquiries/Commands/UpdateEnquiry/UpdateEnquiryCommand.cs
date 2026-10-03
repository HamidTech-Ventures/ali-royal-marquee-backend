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
    string? CustomerName,
    string? CustomerPhone,
    string EventName,
    string? EventType,
    DateOnly PreferredDate,
    DateOnly? AlternativeDate,
    EventShift Shift,
    int GuestCount,
    int BufferCapacity,
    bool PartitionRequired,
    Guid? PreferredVenueId,
    decimal? Budget,
    EnquirySource Source,
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
            .Include(e => e.Customer)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (enquiry == null)
            throw new Exception("Enquiry not found"); // Custom NotFoundException would be better but keeping it simple

        if (enquiry.Customer != null && (!string.IsNullOrWhiteSpace(request.CustomerName) || !string.IsNullOrWhiteSpace(request.CustomerPhone)))
        {
            var newName = !string.IsNullOrWhiteSpace(request.CustomerName) ? request.CustomerName : enquiry.Customer.Name;
            var newPhone = !string.IsNullOrWhiteSpace(request.CustomerPhone) ? request.CustomerPhone : enquiry.Customer.Phone;
            enquiry.Customer.UpdateDetails(newName, newPhone, enquiry.Customer.Email, enquiry.Customer.Tier);
        }

        enquiry.UpdateDetails(
            request.EventName,
            request.EventType,
            request.PreferredDate,
            request.AlternativeDate,
            request.Shift,
            request.GuestCount,
            request.BufferCapacity,
            request.PartitionRequired,
            request.PreferredVenueId,
            request.Budget,
            request.Source,
            request.AssignedToId,
            request.Notes,
            request.EstimatedValue
        );

        await _context.SaveChangesAsync(cancellationToken);
    }
}
