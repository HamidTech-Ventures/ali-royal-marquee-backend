import os
import re

# 1. Update Bookings.tsx stats
bookings_tsx_path = r"c:\My working\HamidTech_Ventures\Clients\marquee-management-system\frontend\src\features\bookings\Bookings.tsx"
with open(bookings_tsx_path, "r", encoding="utf-8") as f:
    bookings_code = f.read()

stats_pattern = r"const stats = \[.*?\];"
new_stats = """const stats = [
    { title: 'Total Bookings', value: bookings.length.toString(), icon: 'event', color: 'bg-primary-container text-on-primary-container' },
    { title: 'Confirmed', value: bookings.filter(b => b.status === 'Confirmed').length.toString(), icon: 'check_circle', color: 'bg-[#d1fae5] text-[#065f46]' },
    { title: 'Pending', value: bookings.filter(b => b.status === 'Pending').length.toString(), icon: 'hourglass_empty', color: 'bg-[#fef3c7] text-[#92400e]' },
    { title: 'Completed', value: bookings.filter(b => b.status === 'Completed').length.toString(), icon: 'task_alt', color: 'bg-blue-100 text-blue-800' },
    { title: 'Cancelled', value: bookings.filter(b => b.status === 'Cancelled').length.toString(), icon: 'cancel', color: 'bg-red-100 text-red-800' }
  ];"""

bookings_code = re.sub(stats_pattern, new_stats, bookings_code, flags=re.DOTALL)
with open(bookings_tsx_path, "w", encoding="utf-8") as f:
    f.write(bookings_code)

# 2. Update BookingDetails.tsx payload
details_tsx_path = r"c:\My working\HamidTech_Ventures\Clients\marquee-management-system\frontend\src\features\bookings\BookingDetails.tsx"
with open(details_tsx_path, "r", encoding="utf-8") as f:
    details_code = f.read()

payload_pattern = r"await bookingsService\.updateBooking\(booking\.id, \{[^\}]+\} as any\);"
new_payload = """await bookingsService.updateBooking(booking.id, {
        id: booking.id,
        venueId: booking.venueId || booking.venue?.id,
        bookingDate: `${bookingDateStr}T00:00:00Z`,
        startTime: `${bookingDateStr}T${startTime}Z`,
        endTime: `${bookingDateStr}T${endTime}Z`,
        guestCount: booking.guests || booking.guestCount || 0,
        totalAmount: booking.totalAmount
      } as any);"""

details_code = re.sub(payload_pattern, new_payload, details_code)
with open(details_tsx_path, "w", encoding="utf-8") as f:
    f.write(details_code)

# 3. Update AddPaymentCommandHandler.cs to confirm pending bookings
add_payment_path = r"c:\My working\HamidTech_Ventures\Clients\marquee-management-system\backend\src\AliRoyalMarquee.Application\Bookings\Commands\AddPayment\AddPaymentCommand.cs"
with open(add_payment_path, "r", encoding="utf-8") as f:
    payment_code = f.read()

if "booking.Confirm();" not in payment_code:
    payment_code = payment_code.replace("booking.AddPayment(payment);", 
"""booking.AddPayment(payment);
        if (booking.Status == BookingStatus.Pending) {
            booking.Confirm();
        }""")
    with open(add_payment_path, "w", encoding="utf-8") as f:
        f.write(payment_code)

print("Done")
