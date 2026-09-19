using MediatR;
using AliRoyalMarquee.Domain.Entities;
using AliRoyalMarquee.Application.Common.Interfaces;

namespace AliRoyalMarquee.Application.Addons.Commands;

public record CreateAddonCommand : IRequest<Guid>
{
    public string Name { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string? Description { get; init; }
    public string Unit { get; init; } = string.Empty;
}

public class CreateAddonCommandHandler : IRequestHandler<CreateAddonCommand, Guid>
{
    private readonly IAppDbContext _context;
    public CreateAddonCommandHandler(IAppDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateAddonCommand request, CancellationToken cancellationToken)
    {
        var item = new Addon
        {
            Name = request.Name,
            Category = request.Category,
            Price = request.Price,
            Description = request.Description,
            Unit = request.Unit
        };
        _context.Addons.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        return item.Id;
    }
}
