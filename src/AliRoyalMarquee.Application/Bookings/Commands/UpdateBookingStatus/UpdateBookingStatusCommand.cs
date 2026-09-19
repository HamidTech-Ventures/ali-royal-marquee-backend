using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using AliRoyalMarquee.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Bookings.Commands.UpdateBookingStatus;

public record UpdateBookingStatusCommand(Guid Id, string Status) : IRequest;

public class UpdateBookingStatusCommandHandler : IRequestHandler<UpdateBookingStatusCommand>
{
    private readonly IAppDbContext _context;

    public UpdateBookingStatusCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateBookingStatusCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .Include(b => b.Customer)
            .Include(b => b.Package)
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (booking == null)
            throw new Exception("Booking not found");

        if (request.Status.Equals("Confirmed", StringComparison.OrdinalIgnoreCase))
        {
            booking.Confirm();

            // Auto-create Event if it doesn't exist
            var existingEvent = await _context.Events.FirstOrDefaultAsync(e => e.BookingId == booking.Id, cancellationToken);
            if (existingEvent == null)
            {
                var newEvent = new Event(
                    booking.Id, 
                    booking.ReferenceNumber, 
                    $"{booking.Customer?.Name} Event", 
                    null // ManagerId will be assigned later
                );
                
                if (booking.Package != null && !string.IsNullOrEmpty(booking.Package.InclusionsJson))
                {
                    try
                    {
                        var inclusions = System.Text.Json.JsonSerializer.Deserialize<string[]>(booking.Package.InclusionsJson);
                        if (inclusions != null)
                        {
                            foreach (var inclusion in inclusions)
                            {
                                newEvent.AddMenuItem(inclusion, "Package Item", booking.GuestCount, "From Booking Package");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Ignore JSON parsing errors for now
                        Console.WriteLine($"Error parsing package inclusions: {ex.Message}");
                    }
                }

                _context.Events.Add(newEvent);
            }
        }
        else if (request.Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase))
        {
            booking.Cancel();
        }
        else if (request.Status.Equals("Completed", StringComparison.OrdinalIgnoreCase))
        {
            booking.Complete();

            // Complete the linked event
            var linkedEvent = await _context.Events.FirstOrDefaultAsync(e => e.BookingId == booking.Id, cancellationToken);
            if (linkedEvent != null)
            {
                linkedEvent.UpdateStatus("Completed");
            }

            // Simple inventory consumption (mock logic: deduct 10 units of all 'Food' items)
            var foodItems = await _context.InventoryItems
                .Where(i => i.Category.ToLower() == "food")
                .ToListAsync(cancellationToken);
            
            foreach(var item in foodItems)
            {
                item.Quantity = Math.Max(0, item.Quantity - 10); // Deduct 10 units, don't go below 0
            }
        }
        else
        {
            throw new Exception("Invalid status transition.");
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
