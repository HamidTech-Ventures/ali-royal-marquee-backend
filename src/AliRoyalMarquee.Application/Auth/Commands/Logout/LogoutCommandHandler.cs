using AliRoyalMarquee.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AliRoyalMarquee.Application.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
{
    private readonly IAppDbContext _context;
    private readonly ITokenService _tokenService;

    public LogoutCommandHandler(IAppDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = _tokenService.HashRefreshToken(request.RefreshToken);

        var session = await _context.RefreshSessions
            .FirstOrDefaultAsync(s => s.TokenHash == tokenHash, cancellationToken);

        if (session != null && !session.IsRevoked)
        {
            session.RevokedAt = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
    }
}
