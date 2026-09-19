using MediatR;
using AliRoyalMarquee.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AliRoyalMarquee.Application.MenuItems.Commands;

public record DeleteMenuItemCommand(Guid Id) : IRequest<bool>;

public class DeleteMenuItemCommandHandler : IRequestHandler<DeleteMenuItemCommand, bool>
{
    private readonly IAppDbContext _context;
    public DeleteMenuItemCommandHandler(IAppDbContext context) => _context = context;

    public async Task<bool> Handle(DeleteMenuItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _context.MenuItems.FindAsync(new object[] { request.Id }, cancellationToken);
        if (item == null) return false;
        _context.MenuItems.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
