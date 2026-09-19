using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using AliRoyalMarquee.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Enquiries.Commands.MarkEnquiryLost;

public record MarkEnquiryLostCommand(Guid Id, string Reason, Guid PerformedById) : IRequest;

public class MarkEnquiryLostCommandHandler : IRequestHandler<MarkEnquiryLostCommand>
{
    private readonly IAppDbContext _context;

    public MarkEnquiryLostCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(MarkEnquiryLostCommand request, CancellationToken cancellationToken)
    {
        var enquiry = await _context.Enquiries
            .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

        if (enquiry == null) throw new Exception("Enquiry not found");

        enquiry.MarkLost(request.Reason);

        var activity = new EnquiryActivity(
            enquiry.Id, 
            EnquiryActivityType.MarkedLost, 
            $"Enquiry marked as Lost. Reason: {request.Reason}", 
            request.PerformedById);
            
        _context.EnquiryActivities.Add(activity);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
