using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.PricingRules.Commands;

public class UpdatePricingRuleCommand : IRequest
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string RuleType { get; init; } = string.Empty;
    public decimal? FlatAmount { get; init; }
    public decimal? PercentageAmount { get; init; }
}

public class UpdatePricingRuleCommandHandler : IRequestHandler<UpdatePricingRuleCommand>
{
    private readonly IAppDbContext _context;
    public UpdatePricingRuleCommandHandler(IAppDbContext context) => _context = context;

    public async Task Handle(UpdatePricingRuleCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.PricingRules.FindAsync(new object[] { request.Id }, cancellationToken);
        if (entity == null) throw new Exception("Not found");
        entity.Name = request.Name;
        entity.RuleType = request.RuleType;
        entity.FlatAmount = request.FlatAmount;
        entity.PercentageAmount = request.PercentageAmount;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
