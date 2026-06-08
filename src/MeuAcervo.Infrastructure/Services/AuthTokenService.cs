using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using MeuAcervo.Application.Abstractions.Auth;
using MeuAcervo.Domain.Entities;
using MeuAcervo.Shared.Auth;
using MeuAcervo.Shared.Configuration;

namespace MeuAcervo.Infrastructure.Services;

public sealed class AuthTokenService : IAuthTokenService
{
    private readonly JwtOptions _jwtOptions;

    public AuthTokenService(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public AccessTokenResult CreateAccessToken(User user, IReadOnlyCollection<string> roles)
    {
        var now = DateTime.UtcNow;
        var expiresAtUtc = now.AddMinutes(_jwtOptions.AccessTokenMinutes);
        var jwtId = Guid.NewGuid().ToString("N");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, jwtId),
            new(JwtClaimNames.TenantId, user.TenantId.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtClaimNames.Username, user.Username)
        };

        claims.AddRange(roles.Select(role => new Claim(JwtClaimNames.Roles, role)));

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));
        var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: now,
            expires: expiresAtUtc,
            signingCredentials: credentials);

        return new AccessTokenResult(
            new JwtSecurityTokenHandler().WriteToken(token),
            jwtId,
            expiresAtUtc);
    }

    public RefreshTokenMaterial CreateRefreshToken()
    {
        var tokenBytes = RandomNumberGenerator.GetBytes(64);
        var refreshToken = Base64UrlEncoder.Encode(tokenBytes);
        var tokenHash = HashRefreshToken(refreshToken);
        var expiresAtUtc = DateTime.UtcNow.AddDays(_jwtOptions.RefreshTokenDays);

        return new RefreshTokenMaterial(refreshToken, tokenHash, expiresAtUtc);
    }

    public string HashRefreshToken(string refreshToken)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));
        return Convert.ToHexString(hashBytes);
    }
}
