using AliRoyalMarquee.Domain.Entities;
using AliRoyalMarquee.Domain.Enums;
using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;
using FluentValidation;

namespace AliRoyalMarquee.Application.Customers.Commands.CreateCustomer;

public record CreateCustomerCommand(string Name, string Phone, string? Email, CustomerTier Tier) : IRequest<Guid>;

public class CreateCustomerValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerValidator()
    {
        RuleFor(v => v.Name).NotEmpty().MaximumLength(100);
        RuleFor(v => v.Phone).NotEmpty().MaximumLength(20);
    }
}

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Guid>
{
    private readonly IAppDbContext _context;

    public CreateCustomerCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = new Customer(request.Name, request.Phone, request.Email, request.Tier);
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(cancellationToken);
        return customer.Id;
    }
}
