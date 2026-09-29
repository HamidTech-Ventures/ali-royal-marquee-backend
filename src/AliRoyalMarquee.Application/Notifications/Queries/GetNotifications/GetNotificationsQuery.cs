using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Application.Notifications.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AliRoyalMarquee.Application.Notifications.Queries.GetNotifications;

public class GetNotificationsQuery : IRequest<List<NotificationDto>>
{
    public Guid UserId { get; set; }
}

public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, List<NotificationDto>>
{
    private readonly IAppDbContext _context;

    public GetNotificationsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await _context.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == request.UserId || n.UserId == null)
            .OrderByDescending(n => n.CreatedAt)
            .Take(50)
            .ToListAsync(cancellationToken);

        return notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            UserId = n.UserId,
            Title = n.Title,
            Message = n.Message,
            Type = n.Type,
            IsRead = n.IsRead,
            ActionUrl = n.ActionUrl,
            CreatedAt = n.CreatedAt
        }).ToList();
    }
}
