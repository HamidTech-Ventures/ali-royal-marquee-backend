using MediatR;
using AliRoyalMarquee.Domain.Entities;
using AliRoyalMarquee.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AliRoyalMarquee.Application.PricingRules.Queries;

public record GetPricingRulesQuery : IRequest<List<PricingRule>>;

public class GetPricingRulesQueryHandler : IRequestHandler<GetPricingRulesQuery, List<PricingRule>>
{
    private readonly IAppDbContext _context;
    public GetPricingRulesQueryHandler(IAppDbContext context) => _context = context;

    public async Task<List<PricingRule>> Handle(GetPricingRulesQuery request, CancellationToken cancellationToken)
    {
        return await _context.PricingRules.AsNoTracking().ToListAsync(cancellationToken);
    }
}
