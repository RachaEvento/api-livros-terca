using AutoMapper;
using FluentValidation;
using MeuAcervo.Application.Abstractions.Auth;
using MeuAcervo.Application.Abstractions.Infrastructure;
using MeuAcervo.Application.Common.Exceptions;
using MeuAcervo.Application.DTOs.Auth;
using MeuAcervo.Application.DTOs.Identity;
using MeuAcervo.Domain.Constants;
using MeuAcervo.Domain.Entities;
using MeuAcervo.Domain.Enums;

namespace MeuAcervo.Application.Services.Auth;

public sealed class AuthService : IAuthService
{
    private readonly IIdentityRepository _identityRepository;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly IAuthTokenService _authTokenService;
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly IMapper _mapper;

    public AuthService(
        IIdentityRepository identityRepository,
        IPasswordHasherService passwordHasherService,
        IAuthTokenService authTokenService,
        IApplicationDbContext applicationDbContext,
        IMapper mapper)
    {
        _identityRepository = identityRepository;
        _passwordHasherService = passwordHasherService;
        _authTokenService = authTokenService;
        _applicationDbContext = applicationDbContext;
        _mapper = mapper;
    }

    public async Task<AuthSessionResponse> RegisterAsync(RegisterRequest request, string? clientIpAddress, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = Normalize(request.Email);
        var normalizedUsername = Normalize(request.Username);
        var tenantSlug = CreateTenantSlug(request.Username);

        if (await _identityRepository.EmailExistsAsync(normalizedEmail, cancellationToken))
        {
            throw new ConflictException("Email is already in use.");
        }

        if (await _identityRepository.UsernameExistsAsync(normalizedUsername, cancellationToken))
        {
            throw new ConflictException("Username is already in use.");
        }

        var tenant = new Tenant
        {
            Name = $"Acervo de {request.DisplayName.Trim()}",
            Slug = tenantSlug,
            Type = TenantType.Personal
        };

        var user = new User
        {
            TenantId = tenant.Id,
            Tenant = tenant,
            Email = request.Email.Trim(),
            NormalizedEmail = normalizedEmail,
            Username = request.Username.Trim(),
            NormalizedUsername = normalizedUsername,
            DisplayName = request.DisplayName.Trim(),
            IsActive = true
        };

        user.PasswordHash = _passwordHasherService.HashPassword(user, request.Password);

        var ownerRole = new Role
        {
            TenantId = tenant.Id,
            Name = DefaultRoleNames.Owner,
            Description = "Owner role for the personal tenant.",
            IsSystemDefault = true
        };

        var permissions = await _identityRepository.GetAllPermissionsAsync(cancellationToken);
        var rolePermissions = permissions.Select(permission => new RolePermission
        {
            RoleId = ownerRole.Id,
            PermissionId = permission.Id
        }).ToArray();

        var userRole = new UserRole
        {
            TenantId = tenant.Id,
            UserId = user.Id,
            RoleId = ownerRole.Id
        };

        var roles = new[] { DefaultRoleNames.Owner };
        var permissionCodes = permissions.Select(permission => permission.Code).ToArray();

        var accessToken = _authTokenService.CreateAccessToken(user, roles);
        var refreshTokenMaterial = _authTokenService.CreateRefreshToken();

        var refreshToken = new RefreshToken
        {
            TenantId = tenant.Id,
            UserId = user.Id,
            TokenHash = refreshTokenMaterial.TokenHash,
            JwtId = accessToken.JwtId,
            ExpiresAtUtc = refreshTokenMaterial.ExpiresAtUtc,
            CreatedByIp = clientIpAddress
        };

        _identityRepository.AddTenant(tenant);
        _identityRepository.AddUser(user);
        _identityRepository.AddRole(ownerRole);
        _identityRepository.AddUserRole(userRole);
        _identityRepository.AddRolePermissions(rolePermissions);
        _identityRepository.AddRefreshToken(refreshToken);

        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return CreateAuthSessionResponse(user, tenant, roles, permissionCodes, accessToken, refreshTokenMaterial);
    }

    public async Task<AuthSessionResponse> LoginAsync(LoginRequest request, string? clientIpAddress, CancellationToken cancellationToken = default)
    {
        var normalizedValue = Normalize(request.EmailOrUsername);
        var user = await _identityRepository.FindByEmailOrUsernameAsync(normalizedValue, cancellationToken);

        if (user is null || !_passwordHasherService.VerifyPassword(user, user.PasswordHash, request.Password))
        {
            throw new UnauthorizedException("Invalid credentials.");
        }

        if (!user.IsActive)
        {
            throw new ForbiddenException("The user account is inactive.");
        }

        var roles = await _identityRepository.GetRoleNamesAsync(user.Id, user.TenantId, cancellationToken);
        var permissionCodes = await _identityRepository.GetPermissionCodesAsync(user.Id, user.TenantId, cancellationToken);

        var accessToken = _authTokenService.CreateAccessToken(user, roles);
        var refreshTokenMaterial = _authTokenService.CreateRefreshToken();

        var refreshToken = new RefreshToken
        {
            TenantId = user.TenantId,
            UserId = user.Id,
            TokenHash = refreshTokenMaterial.TokenHash,
            JwtId = accessToken.JwtId,
            ExpiresAtUtc = refreshTokenMaterial.ExpiresAtUtc,
            CreatedByIp = clientIpAddress
        };

        _identityRepository.AddRefreshToken(refreshToken);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return CreateAuthSessionResponse(user, user.Tenant!, roles, permissionCodes, accessToken, refreshTokenMaterial);
    }

    public async Task<AuthSessionResponse> RefreshAsync(RefreshTokenRequest request, string? clientIpAddress, CancellationToken cancellationToken = default)
    {
        var refreshTokenHash = _authTokenService.HashRefreshToken(request.RefreshToken);
        var storedRefreshToken = await _identityRepository.GetRefreshTokenByHashAsync(refreshTokenHash, cancellationToken);

        if (storedRefreshToken is null)
        {
            throw new UnauthorizedException("Invalid refresh token.");
        }

        if (storedRefreshToken.RevokedAtUtc.HasValue)
        {
            throw new UnauthorizedException("Refresh token has already been revoked.");
        }

        if (storedRefreshToken.ExpiresAtUtc <= DateTime.UtcNow)
        {
            throw new UnauthorizedException("Refresh token has expired.");
        }

        var user = storedRefreshToken.User;
        if (user is null)
        {
            throw new UnauthorizedException("Refresh token is not associated with a valid user.");
        }

        if (!user.IsActive)
        {
            throw new ForbiddenException("The user account is inactive.");
        }

        var roles = await _identityRepository.GetRoleNamesAsync(user.Id, user.TenantId, cancellationToken);
        var permissionCodes = await _identityRepository.GetPermissionCodesAsync(user.Id, user.TenantId, cancellationToken);

        var accessToken = _authTokenService.CreateAccessToken(user, roles);
        var newRefreshTokenMaterial = _authTokenService.CreateRefreshToken();

        storedRefreshToken.RevokedAtUtc = DateTime.UtcNow;
        storedRefreshToken.ReplacedByTokenHash = newRefreshTokenMaterial.TokenHash;

        var replacementToken = new RefreshToken
        {
            TenantId = user.TenantId,
            UserId = user.Id,
            TokenHash = newRefreshTokenMaterial.TokenHash,
            JwtId = accessToken.JwtId,
            ExpiresAtUtc = newRefreshTokenMaterial.ExpiresAtUtc,
            CreatedByIp = clientIpAddress
        };

        _identityRepository.AddRefreshToken(replacementToken);
        await _applicationDbContext.SaveChangesAsync(cancellationToken);

        return CreateAuthSessionResponse(user, user.Tenant!, roles, permissionCodes, accessToken, newRefreshTokenMaterial);
    }

    public async Task<LogoutResponse> LogoutAsync(Guid authenticatedUserId, Guid authenticatedTenantId, LogoutRequest request, CancellationToken cancellationToken = default)
    {
        var refreshTokenHash = _authTokenService.HashRefreshToken(request.RefreshToken);
        var storedRefreshToken = await _identityRepository.GetRefreshTokenByHashAsync(refreshTokenHash, cancellationToken);

        if (storedRefreshToken is null ||
            storedRefreshToken.UserId != authenticatedUserId ||
            storedRefreshToken.TenantId != authenticatedTenantId)
        {
            throw new NotFoundException("Refresh token not found for the authenticated session.");
        }

        if (!storedRefreshToken.RevokedAtUtc.HasValue)
        {
            storedRefreshToken.RevokedAtUtc = DateTime.UtcNow;
            await _applicationDbContext.SaveChangesAsync(cancellationToken);
        }

        return new LogoutResponse(true, storedRefreshToken.RevokedAtUtc ?? DateTime.UtcNow);
    }

    public async Task<CurrentSessionResponse> GetCurrentSessionAsync(Guid authenticatedUserId, Guid authenticatedTenantId, CancellationToken cancellationToken = default)
    {
        var user = await _identityRepository.GetUserByIdAsync(authenticatedUserId, cancellationToken)
                   ?? throw new NotFoundException("Authenticated user was not found.");

        if (user.TenantId != authenticatedTenantId)
        {
            throw new ForbiddenException("Authenticated session is not valid for this tenant.");
        }

        var roles = await _identityRepository.GetRoleNamesAsync(user.Id, user.TenantId, cancellationToken);
        var permissions = await _identityRepository.GetPermissionCodesAsync(user.Id, user.TenantId, cancellationToken);

        return new CurrentSessionResponse(
            _mapper.Map<TenantSummaryResponse>(user.Tenant!),
            _mapper.Map<UserSummaryResponse>(user),
            roles,
            permissions);
    }

    private AuthSessionResponse CreateAuthSessionResponse(
        User user,
        Tenant tenant,
        IReadOnlyCollection<string> roles,
        IReadOnlyCollection<string> permissionCodes,
        AccessTokenResult accessToken,
        RefreshTokenMaterial refreshTokenMaterial)
    {
        return new AuthSessionResponse(
            accessToken.Token,
            refreshTokenMaterial.Token,
            accessToken.ExpiresAtUtc,
            refreshTokenMaterial.ExpiresAtUtc,
            _mapper.Map<TenantSummaryResponse>(tenant),
            _mapper.Map<UserSummaryResponse>(user),
            roles,
            permissionCodes);
    }

    private static string Normalize(string value)
    {
        return value.Trim().ToUpperInvariant();
    }

    private static string CreateTenantSlug(string username)
    {
        return username.Trim().ToLowerInvariant();
    }
}
