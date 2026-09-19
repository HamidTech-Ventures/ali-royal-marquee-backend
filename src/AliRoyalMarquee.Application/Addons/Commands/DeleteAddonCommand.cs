using MediatR;
using AliRoyalMarquee.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AliRoyalMarquee.Application.Addons.Commands;

public record DeleteAddonCommand(Guid Id) : IRequest<bool>;

public class DeleteAddonCommandHandler : IRequestHandler<DeleteAddonCommand, bool>
{
    private readonly IAppDbContext _context;
    public DeleteAddonCommandHandler(IAppDbContext context) => _context = context;

    public async Task<bool> Handle(DeleteAddonCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.Addons.FindAsync(new object[] { request.Id }, cancellationToken);
        if (item == null) return false;
        _context.Addons.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
