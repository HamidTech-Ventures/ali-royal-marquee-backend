using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Events.Commands.UpdateEvent;

public record UpdateEventCommand(Guid Id, string Title, string? ManagerId, int StaffRequired) : IRequest;

public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand>
{
    private readonly IAppDbContext _context;

    public UpdateEventCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateEventCommand request, CancellationToken cancellationToken)
    {
        var e = await _context.Events.FindAsync(new object[] { request.Id }, cancellationToken);
        if (e == null) throw new Exception("Event not found");

        e.UpdateDetails(request.Title, request.ManagerId, request.StaffRequired);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
