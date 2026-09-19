using System;
using AliRoyalMarquee.Domain.Common;

namespace AliRoyalMarquee.Domain.Entities;

public class Expense : BaseEntity
{
    public string Category { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public decimal Amount { get; private set; }
    public DateTime ExpenseDate { get; private set; }
    
    public string Status { get; private set; } = "Pending";

    public Guid? EventId { get; private set; }
    public Event? Event { get; private set; }

    public Guid? VendorId { get; private set; }

    private Expense() { }

    public Expense(string category, string description, decimal amount, DateTime expenseDate, Guid? eventId, Guid? vendorId)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be greater than zero.");

        Category = category;
        Description = description;
        Amount = amount;
        ExpenseDate = expenseDate;
        EventId = eventId;
        VendorId = vendorId;
    }
}
