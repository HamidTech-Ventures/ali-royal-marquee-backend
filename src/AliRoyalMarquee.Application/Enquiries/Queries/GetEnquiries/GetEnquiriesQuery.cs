using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Application.Enquiries.DTOs;
using AliRoyalMarquee.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Enquiries.Queries.GetEnquiries;

public record GetEnquiriesQuery : IRequest<GetEnquiriesResponse>
{
    public string? SearchTerm { get; init; }
    public EnquiryStatus? Status { get; init; }

    public EnquirySource? Source { get; init; }
    public string? EventType { get; init; }
    public Guid? VenueId { get; init; }
    public Guid? AssignedToId { get; init; }
    public DateOnly? PreferredDate { get; init; }
    
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    
    // sorting e.g. "CreatedAtDesc"
    public string? SortBy { get; init; }
}

public class GetEnquiriesResponse
{
    public List<EnquiryDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
}

public class GetEnquiriesQueryHandler : IRequestHandler<GetEnquiriesQuery, GetEnquiriesResponse>
{
    private readonly IAppDbContext _context;

    public GetEnquiriesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<GetEnquiriesResponse> Handle(GetEnquiriesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Enquiries
            .Include(e => e.Customer)
            .Include(e => e.AssignedTo)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.ToLower();
            query = query.Where(e => 
                e.ReferenceNumber.ToLower().Contains(search) ||
                e.Customer.Name.ToLower().Contains(search) ||
                e.Customer.Phone.Contains(search) ||
                e.EventName.ToLower().Contains(search));
        }

        if (request.Status.HasValue)
            query = query.Where(e => e.Status == request.Status.Value);
            

        if (request.Source.HasValue)
            query = query.Where(e => e.Source == request.Source.Value);
            
        if (!string.IsNullOrWhiteSpace(request.EventType))
            query = query.Where(e => e.EventType == request.EventType);
            
        if (request.VenueId.HasValue)
            query = query.Where(e => e.PreferredVenueId == request.VenueId.Value);
            
        if (request.AssignedToId.HasValue)
            query = query.Where(e => e.AssignedToId == request.AssignedToId.Value);
            
        if (request.PreferredDate.HasValue)
            query = query.Where(e => e.PreferredDate == request.PreferredDate.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        // Sorting
        query = request.SortBy switch
        {
            "CreatedAtAsc" => query.OrderBy(e => e.CreatedAt),
            "PreferredDateAsc" => query.OrderBy(e => e.PreferredDate),
            "PreferredDateDesc" => query.OrderByDescending(e => e.PreferredDate),
            "StatusAsc" => query.OrderBy(e => e.Status),
            _ => query.OrderByDescending(e => e.CreatedAt)
        };

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => new EnquiryDto
            {
                Id = e.Id,
                ReferenceNumber = e.ReferenceNumber,
                CustomerId = e.CustomerId,
                CustomerName = e.Customer.Name,
                CustomerPhone = e.Customer.Phone,
                EventName = e.EventName,
                EventType = e.EventType,
                PreferredDate = e.PreferredDate,
                GuestCount = e.GuestCount,
                Source = e.Source,
                Shift = e.Shift,
                BufferCapacity = e.BufferCapacity,
                PartitionRequired = e.PartitionRequired,
                Status = e.Status,
                AssignedToName = e.AssignedTo != null ? e.AssignedTo.FullName : null,
                EstimatedValue = e.EstimatedValue,
                CreatedAt = e.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new GetEnquiriesResponse
        {
            Items = items,
            TotalCount = totalCount
        };
    }
}
