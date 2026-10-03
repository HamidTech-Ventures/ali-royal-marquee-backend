using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Addons.Commands;

public class UpdateAddonCommand : IRequest
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string? Description { get; init; }
    public string Unit { get; init; } = string.Empty;
}

public class UpdateAddonCommandHandler : IRequestHandler<UpdateAddonCommand>
{
    private readonly IAppDbContext _context;
    public UpdateAddonCommandHandler(IAppDbContext context) => _context = context;

    public async Task Handle(UpdateAddonCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Addons.FindAsync(new object[] { request.Id }, cancellationToken);
        if (entity == null) throw new Exception("Not found");
        entity.Name = request.Name;
        entity.Category = request.Category;
        entity.Price = request.Price;
        entity.Description = request.Description;
        entity.Unit = request.Unit;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
