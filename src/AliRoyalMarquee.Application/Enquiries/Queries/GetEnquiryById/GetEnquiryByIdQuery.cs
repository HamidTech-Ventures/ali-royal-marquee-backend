using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Application.Enquiries.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Enquiries.Queries.GetEnquiryById;

public record GetEnquiryByIdQuery(Guid Id) : IRequest<EnquiryDetailDto?>;

public class GetEnquiryByIdQueryHandler : IRequestHandler<GetEnquiryByIdQuery, EnquiryDetailDto?>
{
    private readonly IAppDbContext _context;

    public GetEnquiryByIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<EnquiryDetailDto?> Handle(GetEnquiryByIdQuery request, CancellationToken cancellationToken)
    {
        var enquiry = await _context.Enquiries
            .AsNoTracking()
            .Include(e => e.Customer)
            .Include(e => e.AssignedTo)
            .Include(e => e.PreferredVenue)
            .Include(e => e.FollowUps)
                .ThenInclude(f => f.AssignedTo)
            .Include(e => e.FollowUps)
                .ThenInclude(f => f.CompletedBy)
            .Include(e => e.Quotations)
                .ThenInclude(q => q.Creator)
            .Include(e => e.Activities)
                .ThenInclude(a => a.PerformedBy)
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (enquiry == null) return null;

        return new EnquiryDetailDto
        {
            Id = enquiry.Id,
            ReferenceNumber = enquiry.ReferenceNumber,
            CustomerId = enquiry.CustomerId,
            CustomerName = enquiry.Customer.Name,
            CustomerPhone = enquiry.Customer.Phone,
            EventName = enquiry.EventName,
            EventType = enquiry.EventType,
            PreferredDate = enquiry.PreferredDate,
            GuestCount = enquiry.GuestCount,
            Source = enquiry.Source,
            Shift = enquiry.Shift,
            BufferCapacity = enquiry.BufferCapacity,
            PartitionRequired = enquiry.PartitionRequired,
            Status = enquiry.Status,
            AssignedToName = enquiry.AssignedTo?.FullName,
            EstimatedValue = enquiry.EstimatedValue,
            CreatedAt = enquiry.CreatedAt,

            AlternativeDate = enquiry.AlternativeDate,

            PreferredVenueId = enquiry.PreferredVenueId,
            PreferredVenueName = enquiry.PreferredVenue?.Name,
            Budget = enquiry.Budget,
            AssignedToId = enquiry.AssignedToId,
            Notes = enquiry.Notes,
            LostReason = enquiry.LostReason,

            FollowUps = enquiry.FollowUps.OrderByDescending(f => f.DueDate).Select(f => new EnquiryFollowUpDto
            {
                Id = f.Id,
                DueDate = f.DueDate,
                DueTime = f.DueTime,
                Type = f.Type,
                Notes = f.Notes,
                AssignedToId = f.AssignedToId,
                AssignedToName = f.AssignedTo?.FullName,
                Status = f.Status,
                CompletedAt = f.CompletedAt,
                CompletedByName = f.CompletedBy?.FullName,
                Result = f.Result
            }).ToList(),

            Quotations = enquiry.Quotations.OrderByDescending(q => q.Version).Select(q => new EnquiryQuotationDto
            {
                Id = q.Id,
                QuotationReference = q.QuotationReference,
                Version = q.Version,
                Amount = q.GrandTotal,
                ValidUntil = q.ValidUntil,
                Status = q.Status,
                Notes = q.Notes,
                CreatorName = q.Creator.FullName,
                CreatedAt = q.CreatedAt
            }).ToList(),

            Activities = enquiry.Activities.OrderByDescending(a => a.Timestamp).Select(a => new EnquiryActivityDto
            {
                Id = a.Id,
                Type = a.Type,
                Description = a.Description,
                PerformedByName = a.PerformedBy.FullName,
                Timestamp = a.Timestamp
            }).ToList()
        };
    }
}
