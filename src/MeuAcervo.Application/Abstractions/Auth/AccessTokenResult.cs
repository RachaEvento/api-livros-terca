namespace MeuAcervo.Application.Abstractions.Auth;

public sealed record AccessTokenResult(
    string Token,
    string JwtId,
    DateTime ExpiresAtUtc);
