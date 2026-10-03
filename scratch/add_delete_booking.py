import os

# Create DeleteBookingCommand and Handler
dir_path = r"c:\My working\HamidTech_Ventures\Clients\marquee-management-system\backend\src\AliRoyalMarquee.Application\Bookings\Commands\DeleteBooking"
os.makedirs(dir_path, exist_ok=True)

cmd_code = """using MediatR;

namespace AliRoyalMarquee.Application.Bookings.Commands.DeleteBooking;

public record DeleteBookingCommand(Guid Id) : IRequest;
"""
with open(os.path.join(dir_path, "DeleteBookingCommand.cs"), "w", encoding="utf-8") as f:
    f.write(cmd_code)

handler_code = """using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AliRoyalMarquee.Application.Bookings.Commands.DeleteBooking;

public class DeleteBookingCommandHandler : IRequestHandler<DeleteBookingCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteBookingCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

        if (booking == null)
        {
            throw new Exception($"Booking {request.Id} not found");
        }

        _context.Bookings.Remove(booking);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
"""
with open(os.path.join(dir_path, "DeleteBookingCommandHandler.cs"), "w", encoding="utf-8") as f:
    f.write(handler_code)

# Add to BookingsController.cs
controller_path = r"c:\My working\HamidTech_Ventures\Clients\marquee-management-system\backend\src\AliRoyalMarquee.API\Controllers\BookingsController.cs"
with open(controller_path, "r", encoding="utf-8") as f:
    content = f.read()

# Insert the HttpDelete method
new_method = """
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBooking(Guid id)
    {
        await _mediator.Send(new AliRoyalMarquee.Application.Bookings.Commands.DeleteBooking.DeleteBookingCommand(id));
        return NoContent();
    }
"""

# Insert before the last brace
content = content[:content.rfind('}')] + new_method + "}\n"

with open(controller_path, "w", encoding="utf-8") as f:
    f.write(content)

print("Done")
