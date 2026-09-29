using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;

namespace AliRoyalMarquee.Application.Notifications.Commands.MarkAsRead;

public class MarkNotificationAsReadCommand : IRequest<bool>
{
    public Guid Id { get; set; }
}

public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, bool>
{
    private readonly IAppDbContext _context;

    public MarkNotificationAsReadCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await _context.Notifications.FindAsync(new object[] { request.Id }, cancellationToken);

        if (notification == null)
        {
            return false;
        }

        notification.IsRead = true;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
