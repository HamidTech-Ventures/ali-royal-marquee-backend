using AliRoyalMarquee.Application.Auth.Commands.Login;
using MediatR;

namespace AliRoyalMarquee.Application.Auth.Commands.Refresh;

public record RefreshCommand(string RefreshToken, string? UserAgent, string? IpAddress) : IRequest<LoginResult>;
