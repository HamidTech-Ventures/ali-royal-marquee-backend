using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Enquiries.Commands.DeleteEnquiryQuotation;

public record DeleteEnquiryQuotationCommand(Guid EnquiryId, Guid QuotationId) : IRequest;

public class DeleteEnquiryQuotationCommandHandler : IRequestHandler<DeleteEnquiryQuotationCommand>
{
    private readonly IAppDbContext _context;

    public DeleteEnquiryQuotationCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteEnquiryQuotationCommand request, CancellationToken cancellationToken)
    {
        var enquiry = await _context.Enquiries
            .Include(e => e.Quotations)
            .FirstOrDefaultAsync(e => e.Id == request.EnquiryId, cancellationToken);

        if (enquiry == null)
            throw new Exception("Enquiry not found");

        var quotation = enquiry.Quotations.FirstOrDefault(q => q.Id == request.QuotationId);
        if (quotation == null)
            throw new Exception("Quotation not found");

        _context.EnquiryQuotations.Remove(quotation);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
