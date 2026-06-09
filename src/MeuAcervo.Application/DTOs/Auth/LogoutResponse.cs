namespace MeuAcervo.Application.DTOs.Auth;

public sealed record LogoutResponse(
    bool LoggedOut,
    DateTime LoggedOutAtUtc);
