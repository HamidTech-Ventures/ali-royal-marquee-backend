using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Enums;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AliRoyalMarquee.Application.Enquiries.Commands.UpdateEnquiryFollowUp;

public record UpdateEnquiryFollowUpCommand(
    Guid EnquiryId,
    Guid FollowUpId,
    DateTime DueDate,
    TimeSpan? DueTime,
    FollowUpType Type,
    string? Notes,
    Guid? AssignedToId,
    Guid UpdaterId) : IRequest;

public class UpdateEnquiryFollowUpCommandHandler : IRequestHandler<UpdateEnquiryFollowUpCommand>
{
    private readonly IAppDbContext _context;

    public UpdateEnquiryFollowUpCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateEnquiryFollowUpCommand request, CancellationToken cancellationToken)
    {
        var followUp = await _context.EnquiryFollowUps
            .FirstOrDefaultAsync(f => f.Id == request.FollowUpId && f.EnquiryId == request.EnquiryId, cancellationToken);

        if (followUp == null) throw new Exception("Follow-up not found");

        if (followUp.Status == FollowUpStatus.Completed)
            throw new InvalidOperationException("Cannot update a completed follow-up");

        var newStatus = FollowUpStatus.Pending;
        var now = DateTime.UtcNow;
        if (request.DueDate < now.Date || (request.DueDate == now.Date && request.DueTime.HasValue && request.DueTime.Value <= now.TimeOfDay))
        {
            newStatus = FollowUpStatus.Overdue;
        }

        followUp.UpdateDetails(
            request.DueDate,
            request.DueTime,
            request.Type,
            request.Notes,
            request.AssignedToId,
            newStatus
        );

        await _context.SaveChangesAsync(cancellationToken);
    }
}
