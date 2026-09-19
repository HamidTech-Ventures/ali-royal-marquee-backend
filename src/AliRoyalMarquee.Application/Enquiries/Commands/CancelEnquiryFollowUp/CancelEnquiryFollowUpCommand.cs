using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AliRoyalMarquee.Application.Enquiries.Commands.CancelEnquiryFollowUp;

public record CancelEnquiryFollowUpCommand(Guid EnquiryId, Guid FollowUpId, Guid CancelerId) : IRequest;

public class CancelEnquiryFollowUpCommandHandler : IRequestHandler<CancelEnquiryFollowUpCommand>
{
    private readonly IAppDbContext _context;

    public CancelEnquiryFollowUpCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(CancelEnquiryFollowUpCommand request, CancellationToken cancellationToken)
    {
        var followUp = await _context.EnquiryFollowUps
            .FirstOrDefaultAsync(f => f.Id == request.FollowUpId && f.EnquiryId == request.EnquiryId, cancellationToken);

        if (followUp == null) throw new Exception("Follow-up not found");

        followUp.Cancel();

        await _context.SaveChangesAsync(cancellationToken);
    }
}
