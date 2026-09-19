using AliRoyalMarquee.Application.Auth.Commands.Login;
using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using AliRoyalMarquee.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AliRoyalMarquee.Application.Auth.Commands.Refresh;

public class RefreshCommandHandler : IRequestHandler<RefreshCommand, LoginResult>
{
    private readonly IAppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public RefreshCommandHandler(IAppDbContext context, ITokenService tokenService, IConfiguration configuration)
    {
        _context = context;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    public async Task<LoginResult> Handle(RefreshCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = _tokenService.HashRefreshToken(request.RefreshToken);

        var session = await _context.RefreshSessions
            .Include(s => s.User)
            .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(s => s.TokenHash == tokenHash, cancellationToken);

        if (session == null)
        {
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        if (session.IsRevoked)
        {
            // Security Event: Revoked/Rotated token is being reused. 
            // Invalidate all active sessions for this user as a precaution.
            var activeSessions = await _context.RefreshSessions
                .Where(s => s.UserId == session.UserId && s.RevokedAt == null)
                .ToListAsync(cancellationToken);
            
            foreach (var s in activeSessions)
            {
                s.RevokedAt = DateTimeOffset.UtcNow;
            }
            await _context.SaveChangesAsync(cancellationToken);
            throw new UnauthorizedAccessException("Invalid refresh token.");
        }

        if (session.IsExpired)
        {
            throw new UnauthorizedAccessException("Refresh token expired.");
        }

        if (session.User.Status != UserStatus.Active)
        {
            throw new UnauthorizedAccessException("User is not active.");
        }

        // Revoke the current session (Token Rotation)
        session.RevokedAt = DateTimeOffset.UtcNow;
        session.LastUsedAt = DateTimeOffset.UtcNow;

        var newAccessToken = _tokenService.GenerateAccessToken(session.User);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        var newHashedToken = _tokenService.HashRefreshToken(newRefreshToken);

        var expiresDays = int.Parse(_configuration["Jwt:RefreshTokenDays"] ?? "7");
        var newExpiresAt = DateTimeOffset.UtcNow.AddDays(expiresDays);

        var newSession = new RefreshSession
        {
            UserId = session.UserId,
            TokenHash = newHashedToken,
            ExpiresAt = newExpiresAt,
            UserAgent = request.UserAgent,
            IpAddress = request.IpAddress
        };

        _context.RefreshSessions.Add(newSession);
        
        // Link them up
        session.ReplacedBySession = newSession;

        await _context.SaveChangesAsync(cancellationToken);

        var userDto = new UserDto(session.User.Id, session.User.FullName, session.User.Email, session.User.Role.Name);

        return new LoginResult(newAccessToken, newRefreshToken, newExpiresAt, userDto);
    }
}
