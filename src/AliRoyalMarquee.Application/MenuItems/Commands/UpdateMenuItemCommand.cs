using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.MenuItems.Commands;

public class UpdateMenuItemCommand : IRequest
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string? Description { get; init; }
}

public class UpdateMenuItemCommandHandler : IRequestHandler<UpdateMenuItemCommand>
{
    private readonly IAppDbContext _context;
    public UpdateMenuItemCommandHandler(IAppDbContext context) => _context = context;

    public async Task Handle(UpdateMenuItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.MenuItems.FindAsync(new object[] { request.Id }, cancellationToken);
        if (entity == null) throw new Exception("Not found");
        entity.Name = request.Name;
        entity.Category = request.Category;
        entity.Description = request.Description;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
