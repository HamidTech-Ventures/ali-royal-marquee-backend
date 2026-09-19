using AliRoyalMarquee.Domain.Entities;

namespace AliRoyalMarquee.Application.Common.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    string HashRefreshToken(string token);
}
