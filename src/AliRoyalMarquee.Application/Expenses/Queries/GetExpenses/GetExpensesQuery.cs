using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Expenses.Queries.GetExpenses;

public record ExpenseDto(
    Guid Id, 
    string Category, 
    string Description, 
    decimal Amount, 
    string DateStr, 
    string Status, 
    Guid? EventId, 
    Guid? VendorId
);

public record GetExpensesQuery() : IRequest<List<ExpenseDto>>;

public class GetExpensesQueryHandler : IRequestHandler<GetExpensesQuery, List<ExpenseDto>>
{
    private readonly IAppDbContext _context;

    public GetExpensesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ExpenseDto>> Handle(GetExpensesQuery request, CancellationToken cancellationToken)
    {
        var expenses = await _context.Expenses
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync(cancellationToken);

        return expenses.Select(e => new ExpenseDto(
            e.Id,
            e.Category,
            e.Description,
            e.Amount,
            e.ExpenseDate.ToString("yyyy-MM-dd"),
            e.Status,
            e.EventId,
            e.VendorId
        )).ToList();
    }
}
