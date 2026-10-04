using System;
using System.Collections.Generic;
using AliRoyalMarquee.Domain.Common;
using AliRoyalMarquee.Domain.Enums;

namespace AliRoyalMarquee.Domain.Entities;

public class Booking : BaseEntity
{
    public string ReferenceNumber { get; private set; } = default!;
    
    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; } = default!;

    public Guid VenueId { get; private set; }
    public Venue Venue { get; private set; } = default!;

    public DateTime BookingDate { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }

    public int GuestCount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public BookingStatus Status { get; private set; }
    
    public Guid? PackageId { get; private set; }
    public Package? Package { get; private set; }
    
    public Event? Event { get; private set; }
    
    public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();
    private readonly List<Payment> _payments = new();

    private Booking() { } // EF Core

    public Booking(Guid customerId, Guid venueId, string referenceNumber, DateTime bookingDate, DateTime startTime, DateTime endTime, int guestCount, decimal totalAmount, Guid? packageId = null)
    {
        if (startTime >= endTime)
            throw new ArgumentException("StartTime must be before EndTime");

        CustomerId = customerId;
        VenueId = venueId;
        ReferenceNumber = referenceNumber;
        BookingDate = bookingDate.Date;
        StartTime = startTime;
        EndTime = endTime;
        GuestCount = guestCount;
        TotalAmount = totalAmount;
        Status = BookingStatus.Pending;
        PackageId = packageId;
    }

    
    public void MarkAsDraft()
    {
        Status = BookingStatus.Draft;
    }

    public void Confirm()
    {
        if (Status == BookingStatus.Cancelled)
            throw new InvalidOperationException("Cannot confirm a cancelled booking");
        
        Status = BookingStatus.Confirmed;
    }

    public void Cancel()
    {
        Status = BookingStatus.Cancelled;
    }

    public void Complete()
    {
        if (Status != BookingStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed bookings can be completed");
            
        Status = BookingStatus.Completed;
    }

    public void Update(Guid venueId, DateTime bookingDate, DateTime startTime, DateTime endTime, int guestCount, decimal totalAmount)
    {
        if (startTime >= endTime)
            throw new ArgumentException("StartTime must be before EndTime");

        VenueId = venueId;
        BookingDate = bookingDate.Date;
        StartTime = startTime;
        EndTime = endTime;
        GuestCount = guestCount;
        TotalAmount = totalAmount;
    }

    public void UpdatePackage(Guid? packageId)
    {
        PackageId = packageId;
    }

    public void SetTotalAmount(decimal amount)
    {
        TotalAmount = amount;
    }

    public void AddPayment(Payment payment)
    {
        _payments.Add(payment);
    }
}
