using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Events.Commands.ManageEventTasks;

public record AddEventTaskCommand(Guid EventId, string Title, string? Assignee, string? DueTime) : IRequest<Guid>;

public class AddEventTaskCommandHandler : IRequestHandler<AddEventTaskCommand, Guid>
{
    private readonly IAppDbContext _context;
    public AddEventTaskCommandHandler(IAppDbContext context) => _context = context;

    public async Task<Guid> Handle(AddEventTaskCommand request, CancellationToken cancellationToken)
    {
        var e = await _context.Events.Include(x => x.Tasks).FirstOrDefaultAsync(x => x.Id == request.EventId, cancellationToken);
        if (e == null) throw new Exception("Event not found");

        var task = new EventTask(request.EventId, request.Title, request.Assignee, request.DueTime);
        _context.EventTasks.Add(task);
        
        e.UpdateReadinessScore();

        await _context.SaveChangesAsync(cancellationToken);
        return task.Id;
    }
}

public record UpdateEventTaskCommand(Guid TaskId, string Status, int Progress) : IRequest;

public class UpdateEventTaskCommandHandler : IRequestHandler<UpdateEventTaskCommand>
{
    private readonly IAppDbContext _context;
    public UpdateEventTaskCommandHandler(IAppDbContext context) => _context = context;

    public async Task Handle(UpdateEventTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _context.EventTasks.Include(t => t.Event).ThenInclude(e => e.Tasks).FirstOrDefaultAsync(x => x.Id == request.TaskId, cancellationToken);
        if (task == null) throw new Exception("Task not found");

        task.UpdateStatus(request.Status, request.Progress);
        task.Event.UpdateReadinessScore();

        await _context.SaveChangesAsync(cancellationToken);
    }
}
