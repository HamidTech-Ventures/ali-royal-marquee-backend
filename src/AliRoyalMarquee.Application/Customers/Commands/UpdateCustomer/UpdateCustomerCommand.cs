using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Customers.Commands.UpdateCustomer;

public record UpdateCustomerCommand(Guid Id, string Name, string Phone, string? Email, string Tier) : IRequest;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand>
{
    private readonly IAppDbContext _context;

    public UpdateCustomerCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (customer == null) throw new Exception("Customer not found");

        if (!Enum.TryParse<CustomerTier>(request.Tier, out var tierEnum))
        {
            throw new Exception("Invalid tier");
        }

        customer.UpdateDetails(request.Name, request.Phone, request.Email, tierEnum);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
