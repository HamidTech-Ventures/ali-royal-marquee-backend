using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using AliRoyalMarquee.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Enquiries.Commands.CreateEnquiryFollowUp;

public record CreateEnquiryFollowUpCommand(
    Guid EnquiryId,
    DateTime DueDate,
    TimeSpan? DueTime,
    FollowUpType Type,
    string? Notes,
    Guid? AssignedToId) : IRequest<Guid>;

public class CreateEnquiryFollowUpCommandHandler : IRequestHandler<CreateEnquiryFollowUpCommand, Guid>
{
    private readonly IAppDbContext _context;

    public CreateEnquiryFollowUpCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateEnquiryFollowUpCommand request, CancellationToken cancellationToken)
    {
        var enquiry = await _context.Enquiries
            .FirstOrDefaultAsync(e => e.Id == request.EnquiryId, cancellationToken);

        if (enquiry == null) throw new Exception("Enquiry not found");

        var followUp = new EnquiryFollowUp(
            request.EnquiryId,
            request.DueDate,
            request.DueTime,
            request.Type,
            request.Notes,
            request.AssignedToId
        );

        _context.EnquiryFollowUps.Add(followUp);
        await _context.SaveChangesAsync(cancellationToken);

        return followUp.Id;
    }
}
