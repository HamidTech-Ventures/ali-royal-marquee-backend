using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Customers.Commands.DeleteCustomer;

public class DeleteCustomerCommand : IRequest
{
    public Guid Id { get; set; }
}

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand>
{
    private readonly IAppDbContext _context;

    public DeleteCustomerCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
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
    }
}
