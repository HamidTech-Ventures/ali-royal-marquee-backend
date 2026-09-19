using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using AliRoyalMarquee.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Enquiries.Commands.CompleteEnquiryFollowUp;

public record CompleteEnquiryFollowUpCommand(
    Guid EnquiryId,
    Guid FollowUpId,
    string? Result,
    Guid CompletedById) : IRequest;

public class CompleteEnquiryFollowUpCommandHandler : IRequestHandler<CompleteEnquiryFollowUpCommand>
{
    private readonly IAppDbContext _context;

    public CompleteEnquiryFollowUpCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(CompleteEnquiryFollowUpCommand request, CancellationToken cancellationToken)
    {
        var followUp = await _context.EnquiryFollowUps
            .FirstOrDefaultAsync(f => f.Id == request.FollowUpId && f.EnquiryId == request.EnquiryId, cancellationToken);

        if (followUp == null) throw new Exception("Follow-up not found");

        followUp.Complete(request.CompletedById, request.Result);

        var activity = new EnquiryActivity(
            request.EnquiryId,
            EnquiryActivityType.FollowUpCompleted,
            $"Follow-up completed: {followUp.Type}. Result: {request.Result}",
            request.CompletedById
        );
        _context.EnquiryActivities.Add(activity);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
