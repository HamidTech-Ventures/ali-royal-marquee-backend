using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using AliRoyalMarquee.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Enquiries.Commands.CreateEnquiryActivity;

public record CreateEnquiryActivityCommand(
    Guid EnquiryId,
    EnquiryActivityType Type,
    string Description,
    Guid PerformedById) : IRequest<Guid>;

public class CreateEnquiryActivityCommandHandler : IRequestHandler<CreateEnquiryActivityCommand, Guid>
{
    private readonly IAppDbContext _context;

    public CreateEnquiryActivityCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateEnquiryActivityCommand request, CancellationToken cancellationToken)
    {
        var enquiry = await _context.Enquiries
            .FirstOrDefaultAsync(e => e.Id == request.EnquiryId, cancellationToken);

        if (enquiry == null) throw new Exception("Enquiry not found");

        var activity = new EnquiryActivity(
            request.EnquiryId,
            request.Type,
            request.Description,
            request.PerformedById
        );

        _context.EnquiryActivities.Add(activity);
        await _context.SaveChangesAsync(cancellationToken);

        return activity.Id;
    }
}
