using System;
using AliRoyalMarquee.Domain.Common;
using AliRoyalMarquee.Domain.Enums;

namespace AliRoyalMarquee.Domain.Entities;

public class QuotationLineItem : BaseEntity
{
    public Guid QuotationId { get; private set; }
    public EnquiryQuotation Quotation { get; private set; } = default!;

    public string Category { get; private set; } = default!;
    public QuotationItemType ItemType { get; private set; }
    public string Description { get; private set; } = default!;
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; } = default!;
    public decimal UnitPrice { get; private set; }
    public decimal LineTotal { get; private set; }
    public int SortOrder { get; private set; }
    
    public Guid? PackageId { get; private set; }
    public Package? Package { get; private set; }
    
    public Guid? AddonId { get; private set; }
    public Addon? Addon { get; private set; }

    private QuotationLineItem() { } // EF Core

    public QuotationLineItem(Guid quotationId, QuotationItemType itemType, string category, string description, decimal quantity, string unit, decimal unitPrice, int sortOrder, Guid? packageId = null, Guid? addonId = null)
    {
        QuotationId = quotationId;
        ItemType = itemType;
        Category = category;
        Description = description;
        Quantity = quantity;
        Unit = unit;
        UnitPrice = unitPrice;
        LineTotal = Quantity * UnitPrice; // Authoritative calculation
        SortOrder = sortOrder;
        PackageId = packageId;
        AddonId = addonId;
    }
}
