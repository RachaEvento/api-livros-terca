namespace MeuAcervo.Application.DTOs.Auth;

public sealed record RegisterRequest(
    string Email,
    string Username,
    string DisplayName,
    string Password);
