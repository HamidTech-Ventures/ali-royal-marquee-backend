using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AliRoyalMarquee.Application.Events.Commands.ManageEventStaff;

public record AddEventStaffCommand(Guid EventId, string Name, string Role, Guid? StaffMemberId = null) : IRequest<Guid>;

public class AddEventStaffCommandHandler : IRequestHandler<AddEventStaffCommand, Guid>
{
    private readonly IAppDbContext _context;
    public AddEventStaffCommandHandler(IAppDbContext context) => _context = context;

    public async Task<Guid> Handle(AddEventStaffCommand request, CancellationToken cancellationToken)
    {
        var staff = new EventStaff(request.EventId, request.Name, request.Role, request.StaffMemberId);
        _context.EventStaff.Add(staff);
        await _context.SaveChangesAsync(cancellationToken);
        return staff.Id;
    }
}

public record RemoveEventStaffCommand(Guid EventId, Guid StaffId) : IRequest<bool>;

public class RemoveEventStaffCommandHandler : IRequestHandler<RemoveEventStaffCommand, bool>
{
    private readonly IAppDbContext _context;
    public RemoveEventStaffCommandHandler(IAppDbContext context) => _context = context;

    public async Task<bool> Handle(RemoveEventStaffCommand request, CancellationToken cancellationToken)
    {
        var staff = await _context.EventStaff.FirstOrDefaultAsync(s => s.EventId == request.EventId && s.Id == request.StaffId, cancellationToken);
        if (staff == null) return false;
        
        _context.EventStaff.Remove(staff);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
