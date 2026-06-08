namespace MeuAcervo.Application.DTOs.Auth;

public sealed record LoginRequest(
    string EmailOrUsername,
    string Password);
