using MediatR;

namespace AliRoyalMarquee.Application.Auth.Commands.Login;

public record LoginCommand(string Email, string Password, string? UserAgent, string? IpAddress) : IRequest<LoginResult>;

public record LoginResult(string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt, UserDto User);

public record UserDto(Guid Id, string FullName, string Email, string Role);
