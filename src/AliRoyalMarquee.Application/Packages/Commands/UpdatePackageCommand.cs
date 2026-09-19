using MediatR;
using AliRoyalMarquee.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AliRoyalMarquee.Application.Packages.Commands;

public record UpdatePackageCommand : IRequest<bool>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Status { get; init; } = "Active";
    public int MinGuests { get; init; }
    public int? ProfitMarginTarget { get; init; }
    public string? InternalNotes { get; init; }
    public string? InclusionsJson { get; init; }
}

public class UpdatePackageCommandHandler : IRequestHandler<UpdatePackageCommand, bool>
{
    private readonly IAppDbContext _context;

    public UpdatePackageCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdatePackageCommand request, CancellationToken cancellationToken)
    {
        var package = await _context.Packages
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (package == null) return false;

        package.Name = request.Name;
        package.Type = request.Type;
        package.Price = request.Price;
        package.Status = request.Status;
        package.MinGuests = request.MinGuests;
        package.ProfitMarginTarget = request.ProfitMarginTarget;
        package.InternalNotes = request.InternalNotes;
        package.InclusionsJson = request.InclusionsJson;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
