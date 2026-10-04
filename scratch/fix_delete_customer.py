import sys

file_path = r"c:\My working\HamidTech_Ventures\Clients\marquee-management-system\backend\src\AliRoyalMarquee.Application\Customers\Commands\DeleteCustomer\DeleteCustomerCommand.cs"

with open(file_path, 'r', encoding='utf-8') as f:
    content = f.read()

handler_search = """    public async Task Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Customers
            .Include(c => c.Bookings)
            .Include(c => c.Enquiries)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            // If already deleted, just return successfully so the frontend can sync its state
            return;
        }

        // Manually cascade delete to bypass Restrict constraints if they exist
        if (entity.Bookings != null && entity.Bookings.Any())
        {
            _context.Bookings.RemoveRange(entity.Bookings);
        }

        if (entity.Enquiries != null && entity.Enquiries.Any())
        {
            _context.Enquiries.RemoveRange(entity.Enquiries);
        }

        _context.Customers.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }"""

handler_replace = """    public async Task Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Customers.FindAsync(new object[] { request.Id }, cancellationToken);
        if (entity == null) return;

        var enquiries = await _context.Enquiries.Where(e => e.CustomerId == request.Id).ToListAsync(cancellationToken);
        foreach (var eq in enquiries) {
            _context.EnquiryFollowUps.RemoveRange(await _context.EnquiryFollowUps.Where(f => f.EnquiryId == eq.Id).ToListAsync(cancellationToken));
            _context.EnquiryActivities.RemoveRange(await _context.EnquiryActivities.Where(a => a.EnquiryId == eq.Id).ToListAsync(cancellationToken));
            
            var quotes = await _context.EnquiryQuotations.Where(q => q.EnquiryId == eq.Id).ToListAsync(cancellationToken);
            foreach (var q in quotes) {
                _context.QuotationLineItems.RemoveRange(await _context.QuotationLineItems.Where(l => l.QuotationId == q.Id).ToListAsync(cancellationToken));
            }
            _context.EnquiryQuotations.RemoveRange(quotes);
        }
        _context.Enquiries.RemoveRange(enquiries);

        var bookings = await _context.Bookings.Where(b => b.CustomerId == request.Id).ToListAsync(cancellationToken);
        foreach (var b in bookings) {
            _context.Payments.RemoveRange(await _context.Payments.Where(p => p.BookingId == b.Id).ToListAsync(cancellationToken));
            
            var ev = await _context.Events.FirstOrDefaultAsync(e => e.BookingId == b.Id, cancellationToken);
            if (ev != null) {
                _context.EventTasks.RemoveRange(await _context.EventTasks.Where(t => t.EventId == ev.Id).ToListAsync(cancellationToken));
                _context.EventStaff.RemoveRange(await _context.EventStaff.Where(s => s.EventId == ev.Id).ToListAsync(cancellationToken));
                _context.EventMenuItems.RemoveRange(await _context.EventMenuItems.Where(m => m.EventId == ev.Id).ToListAsync(cancellationToken));
                _context.Expenses.RemoveRange(await _context.Expenses.Where(e => e.EventId == ev.Id).ToListAsync(cancellationToken));
                _context.Events.Remove(ev);
            }
        }
        _context.Bookings.RemoveRange(bookings);

        _context.Customers.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }"""

content = content.replace(handler_search, handler_replace)

with open(file_path, 'w', encoding='utf-8') as f:
    f.write(content)

print("Done updating DeleteCustomerCommand.cs")
