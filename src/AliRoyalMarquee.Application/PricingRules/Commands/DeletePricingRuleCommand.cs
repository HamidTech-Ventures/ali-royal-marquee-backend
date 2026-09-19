using MediatR;
using AliRoyalMarquee.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AliRoyalMarquee.Application.PricingRules.Commands;

public record DeletePricingRuleCommand(Guid Id) : IRequest<bool>;

public class DeletePricingRuleCommandHandler : IRequestHandler<DeletePricingRuleCommand, bool>
{
    private readonly IAppDbContext _context;
    public DeletePricingRuleCommandHandler(IAppDbContext context) => _context = context;

    public async Task<bool> Handle(DeletePricingRuleCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.PricingRules.FindAsync(new object[] { request.Id }, cancellationToken);
        if (item == null) return false;
        _context.PricingRules.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
