using MediatR;
using AliRoyalMarquee.Domain.Entities;
using AliRoyalMarquee.Application.Common.Interfaces;

namespace AliRoyalMarquee.Application.MenuItems.Commands;

public record CreateMenuItemCommand : IRequest<Guid>
{
    public string Name { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Cost { get; init; }
}

public class CreateMenuItemCommandHandler : IRequestHandler<CreateMenuItemCommand, Guid>
{
    private readonly IAppDbContext _context;
    public CreateMenuItemCommandHandler(IAppDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateMenuItemCommand request, CancellationToken cancellationToken)
    {
        var item = new MenuItem
        {
            Name = request.Name,
            Category = request.Category,
            Description = request.Description,
            Cost = request.Cost
        };
        _context.MenuItems.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        return item.Id;
    }
}
