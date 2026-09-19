using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Events.Commands.ManageEventMenu;

public record AddEventMenuItemCommand(Guid EventId, string Name, string Category, int Quantity, string? Notes) : IRequest<Guid>;

public class AddEventMenuItemCommandHandler : IRequestHandler<AddEventMenuItemCommand, Guid>
{
    private readonly IAppDbContext _context;
    public AddEventMenuItemCommandHandler(IAppDbContext context) => _context = context;

    public async Task<Guid> Handle(AddEventMenuItemCommand request, CancellationToken cancellationToken)
    {
        var menuItem = new EventMenuItem(request.EventId, request.Name, request.Category, request.Quantity, request.Notes);
        _context.EventMenuItems.Add(menuItem);
        await _context.SaveChangesAsync(cancellationToken);
        return menuItem.Id;
    }
}
