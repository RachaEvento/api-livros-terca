using MeuAcervo.Domain.Entities;

namespace MeuAcervo.Application.Abstractions.Auth;

public interface IAuthTokenService
{
    AccessTokenResult CreateAccessToken(User user, IReadOnlyCollection<string> roles);

    RefreshTokenMaterial CreateRefreshToken();

    string HashRefreshToken(string refreshToken);
}
