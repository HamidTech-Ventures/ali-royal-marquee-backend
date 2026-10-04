using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Bookings.Commands.GenerateInvoice
{
    public class GenerateInvoiceCommand : IRequest<string>
    {
        public Guid BookingId { get; set; }
    }

    public class GenerateInvoiceCommandHandler : IRequestHandler<GenerateInvoiceCommand, string>
    {
        private readonly ICloudinaryService _cloudinary;
        private readonly IMediator _mediator;

        public GenerateInvoiceCommandHandler(ICloudinaryService cloudinary, IMediator mediator)
        {
            _cloudinary = cloudinary;
            _mediator = mediator;
        }

        public async Task<string> Handle(GenerateInvoiceCommand request, CancellationToken cancellationToken)
        {
            var booking = await _mediator.Send(new AliRoyalMarquee.Application.Bookings.Queries.GetBookingById.GetBookingByIdQuery(request.BookingId), cancellationToken);
            if (booking == null)
            {
                throw new System.Exception("Booking not found");
            }
            return await _cloudinary.GenerateAndUploadInvoiceAsync(booking);
        }
    }
}
