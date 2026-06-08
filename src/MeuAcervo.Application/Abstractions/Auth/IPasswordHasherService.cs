using MeuAcervo.Domain.Entities;

namespace MeuAcervo.Application.Abstractions.Auth;

public interface IPasswordHasherService
{
    string HashPassword(User user, string password);

    bool VerifyPassword(User user, string hashedPassword, string providedPassword);
}
