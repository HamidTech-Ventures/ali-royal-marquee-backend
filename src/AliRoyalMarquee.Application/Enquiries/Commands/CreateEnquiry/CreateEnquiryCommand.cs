using AliRoyalMarquee.Domain.Entities;
using AliRoyalMarquee.Domain.Enums;
using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AliRoyalMarquee.Application.Enquiries.Commands.CreateEnquiry;

public record CreateEnquiryCommand(
    Guid? CustomerId,
    string? CustomerName, 
    string? CustomerPhone,
    string EventName, 
    string? EventType, 
    DateOnly PreferredDate, 
    DateOnly? AlternativeDate,
    TimeOnly? PreferredStartTime,
    TimeOnly? PreferredEndTime,
    int GuestCount,
    Guid? PreferredVenueId,
    decimal? Budget,
    string? Notes,
    EnquirySource Source = EnquirySource.WalkIn,
    EnquiryPriority Priority = EnquiryPriority.Warm,
    Guid? AssignedToId = null) : IRequest<Guid>;

public class CreateEnquiryValidator : AbstractValidator<CreateEnquiryCommand>
{
    public CreateEnquiryValidator()
    {
        RuleFor(v => v).Must(v => v.CustomerId.HasValue || (!string.IsNullOrWhiteSpace(v.CustomerName) && !string.IsNullOrWhiteSpace(v.CustomerPhone)))
            .WithMessage("Either an existing Customer must be selected, or a new Customer Name and Phone must be provided.");
        RuleFor(v => v.EventName).NotEmpty().MaximumLength(100);
        RuleFor(v => v.GuestCount).GreaterThan(0);
        RuleFor(v => v.PreferredEndTime).GreaterThan(v => v.PreferredStartTime)
            .When(v => v.PreferredStartTime.HasValue && v.PreferredEndTime.HasValue)
            .WithMessage("End time must be after start time.");
    }
}

public class CreateEnquiryCommandHandler : IRequestHandler<CreateEnquiryCommand, Guid>
{
    private readonly IAppDbContext _context;

    public CreateEnquiryCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateEnquiryCommand request, CancellationToken cancellationToken)
    {
        var referenceNumber = $"ENQ-{DateTime.UtcNow.Ticks.ToString().Substring(10)}";

        Guid customerId;

        if (request.CustomerId.HasValue)
        {
            customerId = request.CustomerId.Value;
        }
        else
        {
            // Try to find an existing customer by phone
            var existingCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Phone == request.CustomerPhone, cancellationToken);
            
            if (existingCustomer != null)
            {
                customerId = existingCustomer.Id;
            }
            else
            {
                // Create a new Customer
                var customer = new Customer(request.CustomerName!, request.CustomerPhone!, null, CustomerTier.Standard);
                _context.Customers.Add(customer);
                customerId = customer.Id;
            }
        }

        var enquiry = new Enquiry(customerId, referenceNumber, request.EventName, request.EventType, request.PreferredDate, request.GuestCount, request.Source, request.Priority);
        
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
            null);
        
        _context.Enquiries.Add(enquiry);
        await _context.SaveChangesAsync(cancellationToken);
        
        return enquiry.Id;
    }
}
