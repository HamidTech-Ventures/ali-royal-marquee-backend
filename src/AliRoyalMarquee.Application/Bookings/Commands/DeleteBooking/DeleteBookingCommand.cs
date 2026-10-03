using MediatR;

namespace AliRoyalMarquee.Application.Bookings.Commands.DeleteBooking;

public record DeleteBookingCommand(Guid Id) : IRequest;
