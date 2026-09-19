using MediatR;
using AliRoyalMarquee.Domain.Entities;
using AliRoyalMarquee.Application.Common.Interfaces;

namespace AliRoyalMarquee.Application.PricingRules.Commands;

public record CreatePricingRuleCommand : IRequest<Guid>
{
    public string Name { get; init; } = string.Empty;
    public string RuleType { get; init; } = string.Empty;
    public decimal? FlatAmount { get; init; }
    public decimal? PercentageAmount { get; init; }
}

public class CreatePricingRuleCommandHandler : IRequestHandler<CreatePricingRuleCommand, Guid>
{
    private readonly IAppDbContext _context;
    public CreatePricingRuleCommandHandler(IAppDbContext context) => _context = context;

    public async Task<Guid> Handle(CreatePricingRuleCommand request, CancellationToken cancellationToken)
    {
        var item = new PricingRule
        {
            Name = request.Name,
            RuleType = request.RuleType,
            FlatAmount = request.FlatAmount,
            PercentageAmount = request.PercentageAmount
        };
        _context.PricingRules.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        return item.Id;
    }
}
