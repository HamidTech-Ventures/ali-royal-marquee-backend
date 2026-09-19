using MediatR;
using AliRoyalMarquee.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AliRoyalMarquee.Application.Packages.Commands;

public record DeletePackageCommand(Guid Id) : IRequest<bool>;

public class DeletePackageCommandHandler : IRequestHandler<DeletePackageCommand, bool>
{
    private readonly IAppDbContext _context;

    public DeletePackageCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeletePackageCommand request, CancellationToken cancellationToken)
    {
        var package = await _context.Packages
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (package == null) return false;

        _context.Packages.Remove(package);
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}
