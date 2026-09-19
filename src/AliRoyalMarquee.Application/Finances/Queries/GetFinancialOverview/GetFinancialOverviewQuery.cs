using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Finances.Queries.GetFinancialOverview;

public record MonthlyDataDto(string Month, decimal Revenue, decimal Expenses, decimal Profit);

public record FinancialOverviewDto(
    decimal TotalRevenue, 
    decimal TotalExpenses, 
    decimal NetProfit, 
    decimal TotalPaid, 
    decimal Receivables,
    List<MonthlyDataDto> MonthlyData
);

public record GetFinancialOverviewQuery() : IRequest<FinancialOverviewDto>;

public class GetFinancialOverviewQueryHandler : IRequestHandler<GetFinancialOverviewQuery, FinancialOverviewDto>
{
    private readonly IAppDbContext _context;

    public GetFinancialOverviewQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<FinancialOverviewDto> Handle(GetFinancialOverviewQuery request, CancellationToken cancellationToken)
    {
        var confirmedBookings = await _context.Bookings
            .Where(b => b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed)
            .ToListAsync(cancellationToken);

        var allPayments = await _context.Payments
            .Where(p => p.Status == PaymentStatus.Completed)
            .ToListAsync(cancellationToken);

        var allExpenses = await _context.Expenses
            .ToListAsync(cancellationToken);

        var totalRevenue = confirmedBookings.Sum(b => b.TotalAmount);
        var totalExpenses = allExpenses.Sum(e => e.Amount);
        var netProfit = totalRevenue - totalExpenses;
        var totalPaid = allPayments.Sum(p => p.Amount);
        var receivables = totalRevenue - totalPaid;

        var monthlyData = new List<MonthlyDataDto>();
        
        var today = DateTime.UtcNow;
        for (int i = 5; i >= 0; i--)
        {
            var targetMonth = today.AddMonths(-i);
            
            var monthRevenue = confirmedBookings
                .Where(b => b.BookingDate.Year == targetMonth.Year && b.BookingDate.Month == targetMonth.Month)
                .Sum(b => b.TotalAmount);
                
            var monthExpenses = allExpenses
                .Where(e => e.ExpenseDate.Year == targetMonth.Year && e.ExpenseDate.Month == targetMonth.Month)
                .Sum(e => e.Amount);
                
            var monthProfit = monthRevenue - monthExpenses;
            
            monthlyData.Add(new MonthlyDataDto(
                targetMonth.ToString("MMM"),
                monthRevenue,
                monthExpenses,
                monthProfit
            ));
        }

        return new FinancialOverviewDto(
            totalRevenue,
            totalExpenses,
            netProfit,
            totalPaid,
            receivables,
            monthlyData
        );
    }
}
