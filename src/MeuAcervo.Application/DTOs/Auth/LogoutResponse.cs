namespace MeuAcervo.Application.DTOs.Auth;

public sealed record LogoutResponse(
    bool Revoked,
    DateTime RevokedAtUtc);
