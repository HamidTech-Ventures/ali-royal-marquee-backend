using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AliRoyalMarquee.Application.Auth.Commands.ChangePassword;

public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Unit>
{
    private readonly IAppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public ChangePasswordCommandHandler(IAppDbContext context, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found.");
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.CurrentPassword);
        
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("Incorrect current password.");
        }

        user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);

        // Revoke all existing sessions
        var activeSessions = await _context.RefreshSessions
            .Where(s => s.UserId == user.Id && s.RevokedAt == null)
            .ToListAsync(cancellationToken);
            
        foreach (var s in activeSessions)
        {
            s.RevokedAt = DateTimeOffset.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
