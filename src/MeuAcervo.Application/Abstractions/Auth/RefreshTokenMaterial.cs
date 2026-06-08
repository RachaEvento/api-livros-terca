namespace MeuAcervo.Application.Abstractions.Auth;

public sealed record RefreshTokenMaterial(
    string Token,
    string TokenHash,
    DateTime ExpiresAtUtc);
