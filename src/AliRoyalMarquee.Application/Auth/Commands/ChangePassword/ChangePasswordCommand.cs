using MediatR;

namespace AliRoyalMarquee.Application.Auth.Commands.ChangePassword;

public record ChangePasswordCommand(Guid UserId, string CurrentPassword, string NewPassword) : IRequest<Unit>;
