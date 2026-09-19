using MediatR;

namespace AliRoyalMarquee.Application.Auth.Commands.Logout;

public record LogoutCommand(string RefreshToken) : IRequest<Unit>;
