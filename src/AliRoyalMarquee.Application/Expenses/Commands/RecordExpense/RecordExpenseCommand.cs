using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Expenses.Commands.RecordExpense;

public record RecordExpenseCommand(string Category, string Description, decimal Amount, DateTime ExpenseDate, Guid? EventId, Guid? VendorId) : IRequest<Guid>;

public class RecordExpenseCommandHandler : IRequestHandler<RecordExpenseCommand, Guid>
{
    private readonly IAppDbContext _context;

    public RecordExpenseCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(RecordExpenseCommand request, CancellationToken cancellationToken)
    {
        var expense = new Expense(
            request.Category,
            request.Description,
            request.Amount,
            request.ExpenseDate,
            request.EventId,
            request.VendorId
        );

        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync(cancellationToken);

        return expense.Id;
    }
}
