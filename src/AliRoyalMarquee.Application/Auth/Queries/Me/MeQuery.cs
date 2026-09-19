using MediatR;

namespace AliRoyalMarquee.Application.Auth.Queries.Me;

public record MeQuery(Guid UserId) : IRequest<MeResult>;

public record MeResult(Guid Id, string FullName, string Email, string Role, string Status, List<string> Permissions);
