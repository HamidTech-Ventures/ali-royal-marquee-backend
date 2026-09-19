using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using AliRoyalMarquee.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Enquiries.Commands.CreateEnquiryQuotation;

public record CreateQuotationLineItemDto(
    string Category,
    string Description,
    decimal Quantity,
    string Unit,
    decimal UnitPrice,
    int SortOrder
);

public record CreateEnquiryQuotationCommand(
    Guid EnquiryId,
    List<CreateQuotationLineItemDto> LineItems,
    decimal DiscountAmount,
    decimal ServiceChargeAmount,
    decimal TaxAmount,
    DateTime? ValidUntil,
    string? Notes,
    Guid CreatorId) : IRequest<Guid>;

public class CreateEnquiryQuotationCommandHandler : IRequestHandler<CreateEnquiryQuotationCommand, Guid>
{
    private readonly IAppDbContext _context;

    public CreateEnquiryQuotationCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateEnquiryQuotationCommand request, CancellationToken cancellationToken)
    {
        var enquiry = await _context.Enquiries
            .Include(e => e.Quotations)
            .FirstOrDefaultAsync(e => e.Id == request.EnquiryId, cancellationToken);

        if (enquiry == null) throw new Exception("Enquiry not found");

        var currentVersion = enquiry.Quotations.Count > 0 ? enquiry.Quotations.Max(q => q.Version) : 0;
        var nextVersion = currentVersion + 1;
        var quotationReference = $"QT-{enquiry.ReferenceNumber.Replace("ENQ-", "")}-V{nextVersion}";

        var quotation = new EnquiryQuotation(
            request.EnquiryId,
            quotationReference,
            nextVersion,
            null, // previousQuotationId
            request.ValidUntil,
            request.Notes,
            request.CreatorId
        );

        foreach (var li in request.LineItems)
        {
            quotation.AddLineItem(li.Category, li.Description, li.Quantity, li.Unit, li.UnitPrice, li.SortOrder);
        }

        quotation.SetChargesAndDiscounts(request.DiscountAmount, request.ServiceChargeAmount, request.TaxAmount);

        _context.EnquiryQuotations.Add(quotation);
        
        var activity = new EnquiryActivity(
            request.EnquiryId,
            EnquiryActivityType.NoteAdded, // You could use QuotationSent if that fits better
            $"Quotation {quotationReference} created for amount {quotation.GrandTotal:C}",
            request.CreatorId
        );
        _context.EnquiryActivities.Add(activity);

        await _context.SaveChangesAsync(cancellationToken);

        return quotation.Id;
    }
}
