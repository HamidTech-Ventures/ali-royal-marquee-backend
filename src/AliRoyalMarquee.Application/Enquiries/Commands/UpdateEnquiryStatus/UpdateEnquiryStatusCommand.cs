using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using AliRoyalMarquee.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Enquiries.Commands.UpdateEnquiryStatus;

public record UpdateEnquiryStatusCommand(Guid Id, EnquiryStatus Status, Guid PerformedById) : IRequest;

public class UpdateEnquiryStatusCommandHandler : IRequestHandler<UpdateEnquiryStatusCommand>
{
    private readonly IAppDbContext _context;

    public UpdateEnquiryStatusCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateEnquiryStatusCommand request, CancellationToken cancellationToken)
    {
        var enquiry = await _context.Enquiries
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (enquiry == null) throw new Exception("Enquiry not found");

        var oldStatus = enquiry.Status;
        enquiry.UpdateStatus(request.Status);

        if (oldStatus != request.Status)
        {
            var activity = new EnquiryActivity(
                enquiry.Id, 
                EnquiryActivityType.StatusChanged, 
                $"Status changed from {oldStatus} to {request.Status}", 
                request.PerformedById);
            _context.EnquiryActivities.Add(activity);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
