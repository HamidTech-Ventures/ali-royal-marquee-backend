using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Enquiries.Queries.GetEnquiryLifecycle;

public record EnquiryLifecycleDto(
    int New,
    int Contacted,
    int Qualified,
    int VisitScheduled,
    int QuotationSent,
    int Negotiation,
    int Converted,
    int Lost
);

public record GetEnquiryLifecycleQuery(string Scope) : IRequest<EnquiryLifecycleDto>;

public class GetEnquiryLifecycleQueryHandler : IRequestHandler<GetEnquiryLifecycleQuery, EnquiryLifecycleDto>
{
    private readonly IAppDbContext _context;

    public GetEnquiryLifecycleQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<EnquiryLifecycleDto> Handle(GetEnquiryLifecycleQuery request, CancellationToken cancellationToken)
    {
        var scope = request.Scope?.ToLowerInvariant();
        if (scope != "all" && scope != "hot")
        {
            throw new FluentValidation.ValidationException(new[] { 
                new FluentValidation.Results.ValidationFailure("Scope", "Supported values are 'all' and 'hot'.") 
            });
        }

        var query = _context.Enquiries.AsNoTracking();

        if (scope == "hot")
        {
            query = query.Where(e => e.Priority == EnquiryPriority.Hot);
        }

        var counts = await query
            .GroupBy(e => e.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(k => k.Status, v => v.Count, cancellationToken);

        int GetCount(EnquiryStatus status) => counts.TryGetValue(status, out var count) ? count : 0;

        return new EnquiryLifecycleDto(
            GetCount(EnquiryStatus.New),
            GetCount(EnquiryStatus.Contacted),
            GetCount(EnquiryStatus.Qualified),
            GetCount(EnquiryStatus.Scheduled),
            GetCount(EnquiryStatus.Quoted),
            GetCount(EnquiryStatus.Negotiating),
            GetCount(EnquiryStatus.Converted),
            GetCount(EnquiryStatus.Lost)
        );
    }
}
