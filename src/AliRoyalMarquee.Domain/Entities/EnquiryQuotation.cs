using System;
using AliRoyalMarquee.Domain.Common;
using AliRoyalMarquee.Domain.Enums;

namespace AliRoyalMarquee.Domain.Entities;

public class EnquiryQuotation : BaseEntity
{
    public Guid EnquiryId { get; private set; }
    public Enquiry Enquiry { get; private set; } = default!;

    public string QuotationReference { get; private set; } = default!;
    public int Version { get; private set; }
    public Guid? PreviousQuotationId { get; private set; }
    public EnquiryQuotation? PreviousQuotation { get; private set; }
    
    // Financials
    public decimal Subtotal { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal ServiceChargeAmount { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal GrandTotal { get; private set; }

    public DateTime? ValidUntil { get; private set; }
    public QuotationStatus Status { get; private set; }
    public string? Notes { get; private set; }

    public Guid CreatorId { get; private set; }
    public User Creator { get; private set; } = default!;

    private readonly List<QuotationLineItem> _lineItems = new();
    public IReadOnlyCollection<QuotationLineItem> LineItems => _lineItems.AsReadOnly();

    private EnquiryQuotation() { } // EF Core

    public EnquiryQuotation(
        Guid enquiryId, 
        string quotationReference, 
        int version, 
        Guid? previousQuotationId,
        DateTime? validUntil, 
        string? notes, 
        Guid creatorId)
    {
        EnquiryId = enquiryId;
        QuotationReference = quotationReference;
        Version = version;
        PreviousQuotationId = previousQuotationId;
        ValidUntil = validUntil?.Date;
        Notes = notes;
        CreatorId = creatorId;
        Status = QuotationStatus.Draft;
    }

    public void AddLineItem(string category, string description, decimal quantity, string unit, decimal unitPrice, int sortOrder, Guid? packageId = null, Guid? addonId = null)
    {
        _lineItems.Add(new QuotationLineItem(Id, category, description, quantity, unit, unitPrice, sortOrder, packageId, addonId));
        RecalculateTotals();
    }

    public void SetChargesAndDiscounts(decimal discountAmount, decimal serviceChargeAmount, decimal taxAmount)
    {
        DiscountAmount = discountAmount;
        ServiceChargeAmount = serviceChargeAmount;
        TaxAmount = taxAmount;
        RecalculateTotals();
    }

    private void RecalculateTotals()
    {
        Subtotal = _lineItems.Sum(li => li.LineTotal);
        GrandTotal = Subtotal - DiscountAmount + ServiceChargeAmount + TaxAmount;
    }

    public void UpdateStatus(QuotationStatus status)
    {
        Status = status;
    }
}
