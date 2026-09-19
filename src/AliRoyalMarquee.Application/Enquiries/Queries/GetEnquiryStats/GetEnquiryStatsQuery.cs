using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Enquiries.Queries.GetEnquiryStats;

public record EnquiryStatsDto(
    int TotalEnquiries,
    int NewThisWeek,
    int FollowUpsDue,
    int HotLeads,
    double ConversionRate,
    decimal EstimatedPipelineValue
);

public record GetEnquiryStatsQuery : IRequest<EnquiryStatsDto>;

public class GetEnquiryStatsQueryHandler : IRequestHandler<GetEnquiryStatsQuery, EnquiryStatsDto>
{
    private readonly IAppDbContext _context;

    public GetEnquiryStatsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<EnquiryStatsDto> Handle(GetEnquiryStatsQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var startOfWeek = now.Date.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday); // Assuming week starts on Monday

        var allEnquiries = await _context.Enquiries
            .Include(e => e.Quotations)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var totalEnquiries = allEnquiries.Count;
        
        var newThisWeek = allEnquiries.Count(e => e.CreatedAt >= startOfWeek);
        
        var hotLeads = allEnquiries.Count(e => e.Priority == EnquiryPriority.Hot && e.Status != EnquiryStatus.Converted && e.Status != EnquiryStatus.Lost);

        var converted = allEnquiries.Count(e => e.Status == EnquiryStatus.Converted);
        var lost = allEnquiries.Count(e => e.Status == EnquiryStatus.Lost);
        var conversionRate = (converted + lost) > 0 ? Math.Round((double)converted / (converted + lost) * 100, 2) : 0;

        var activeEnquiries = allEnquiries.Where(e => e.Status != EnquiryStatus.Converted && e.Status != EnquiryStatus.Lost).ToList();
        var estimatedPipelineValue = activeEnquiries.Sum(e => 
        {
            var activeQuotation = e.Quotations
                .Where(q => q.Status != QuotationStatus.Expired && q.Status != QuotationStatus.Rejected)
                .OrderByDescending(q => q.Version)
                .FirstOrDefault();
                
            return activeQuotation?.GrandTotal ?? e.Budget ?? 0;
        });

        var today = now.Date;
        var currentTime = now.TimeOfDay;
        var pendingFollowUps = await _context.EnquiryFollowUps
            .CountAsync(f => f.Status == FollowUpStatus.Pending && 
                             (f.DueDate < today || (f.DueDate == today && (f.DueTime == null || f.DueTime <= currentTime))),
                        cancellationToken);

        return new EnquiryStatsDto(
            totalEnquiries,
            newThisWeek,
            pendingFollowUps,
            hotLeads,
            conversionRate,
            estimatedPipelineValue
        );
    }
}
