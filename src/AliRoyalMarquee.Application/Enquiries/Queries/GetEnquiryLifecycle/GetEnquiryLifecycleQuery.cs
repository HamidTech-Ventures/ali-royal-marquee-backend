using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Enquiries.Queries.GetEnquiryLifecycle;

public record EnquiryLifecycleDto(
    int Inquiry,
    int SiteVisit,
    int TokenReceived,
    int AdvancePaid,
    int Cancelled
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
        if (scope != "all")
        {
            throw new FluentValidation.ValidationException(new[] { 
                new FluentValidation.Results.ValidationFailure("Scope", "Supported values are 'all'.") 
            });
        }

        var query = _context.Enquiries.AsNoTracking();

        var counts = await query
            .GroupBy(e => e.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(k => k.Status, v => v.Count, cancellationToken);

        int GetCount(EnquiryStatus status) => counts.TryGetValue(status, out var count) ? count : 0;

        return new EnquiryLifecycleDto(
            GetCount(EnquiryStatus.Inquiry),
            GetCount(EnquiryStatus.SiteVisit),
            GetCount(EnquiryStatus.TokenReceived),
            GetCount(EnquiryStatus.AdvancePaid),
            GetCount(EnquiryStatus.Cancelled)
        );
    }
}
