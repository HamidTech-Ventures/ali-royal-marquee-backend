using AliRoyalMarquee.Application.Common.Interfaces;
using AliRoyalMarquee.Domain.Entities;
using AliRoyalMarquee.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AliRoyalMarquee.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IAppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _configuration;

    public LoginCommandHandler(
        IAppDbContext context,
        IPasswordHasher<User> passwordHasher,
        ITokenService tokenService,
        IConfiguration configuration)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _configuration = configuration;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user == null || user.Status != UserStatus.Active)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        if (user.LockoutUntil.HasValue && user.LockoutUntil.Value > DateTimeOffset.UtcNow)
        {
            throw new UnauthorizedAccessException("Account is locked. Try again later.");
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            user.FailedLoginCount++;
            if (user.FailedLoginCount >= 5)
            {
                user.LockoutUntil = DateTimeOffset.UtcNow.AddMinutes(15);
            }
            await _context.SaveChangesAsync(cancellationToken);
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        // Success
        user.FailedLoginCount = 0;
        user.LockoutUntil = null;
        user.LastLoginAt = DateTimeOffset.UtcNow;

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var hashedRefreshToken = _tokenService.HashRefreshToken(refreshToken);
        
        var expiresDays = int.Parse(_configuration["Jwt:RefreshTokenDays"] ?? "7");
        var expiresAt = DateTimeOffset.UtcNow.AddDays(expiresDays);

        var session = new RefreshSession
        {
            UserId = user.Id,
            TokenHash = hashedRefreshToken,
            ExpiresAt = expiresAt,
            UserAgent = request.UserAgent,
            IpAddress = request.IpAddress
        };

        _context.RefreshSessions.Add(session);
        await _context.SaveChangesAsync(cancellationToken);

        return new LoginResult(
            accessToken, 
            refreshToken, 
            expiresAt, 
            new UserDto(user.Id, user.FullName, user.Email, user.Role.Name));
    }
}
