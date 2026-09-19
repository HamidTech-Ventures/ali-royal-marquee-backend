using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using AliRoyalMarquee.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Enquiries.Commands.CreateQuotationRevision;

public record CreateQuotationRevisionCommand(Guid EnquiryId, Guid QuotationId, Guid CreatorId) : IRequest<Guid>;

public class CreateQuotationRevisionCommandHandler : IRequestHandler<CreateQuotationRevisionCommand, Guid>
{
    private readonly IAppDbContext _context;

    public CreateQuotationRevisionCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateQuotationRevisionCommand request, CancellationToken cancellationToken)
    {
        var enquiry = await _context.Enquiries
            .Include(e => e.Quotations)
                .ThenInclude(q => q.LineItems)
            .FirstOrDefaultAsync(e => e.Id == request.EnquiryId, cancellationToken);

        if (enquiry == null) throw new Exception("Enquiry not found");

        var originalQuotation = enquiry.Quotations.FirstOrDefault(q => q.Id == request.QuotationId);
        if (originalQuotation == null) throw new Exception("Quotation not found");

        var nextVersion = enquiry.Quotations.Where(q => q.QuotationReference == originalQuotation.QuotationReference).Max(q => q.Version) + 1;

        var newQuotation = new EnquiryQuotation(
            request.EnquiryId,
            originalQuotation.QuotationReference,
            nextVersion,
            originalQuotation.Id,
            originalQuotation.ValidUntil,
            originalQuotation.Notes,
            request.CreatorId
        );

        foreach (var li in originalQuotation.LineItems)
        {
            newQuotation.AddLineItem(li.Category, li.Description, li.Quantity, li.Unit, li.UnitPrice, li.SortOrder);
        }

        newQuotation.SetChargesAndDiscounts(originalQuotation.DiscountAmount, originalQuotation.ServiceChargeAmount, originalQuotation.TaxAmount);

        _context.EnquiryQuotations.Add(newQuotation);
        
        var activity = new EnquiryActivity(
            request.EnquiryId,
            EnquiryActivityType.NoteAdded,
            $"Quotation revision {newQuotation.QuotationReference}-v{newQuotation.Version} created based on v{originalQuotation.Version}",
            request.CreatorId
        );
        _context.EnquiryActivities.Add(activity);

        await _context.SaveChangesAsync(cancellationToken);

        return newQuotation.Id;
    }
}
