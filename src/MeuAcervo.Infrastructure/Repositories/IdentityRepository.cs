using Microsoft.EntityFrameworkCore;
using MeuAcervo.Application.Abstractions.Auth;
using MeuAcervo.Domain.Entities;
using MeuAcervo.Infrastructure.Data;

namespace MeuAcervo.Infrastructure.Repositories;

public sealed class IdentityRepository : IIdentityRepository
{
    private readonly ApplicationDbContext _dbContext;

    public IdentityRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users.AnyAsync(user => user.NormalizedEmail == normalizedEmail, cancellationToken);
    }

    public Task<bool> UsernameExistsAsync(string normalizedUsername, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users.AnyAsync(user => user.NormalizedUsername == normalizedUsername, cancellationToken);
    }

    public Task<User?> FindByEmailOrUsernameAsync(string normalizedValue, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users
            .Include(user => user.Tenant)
            .FirstOrDefaultAsync(
                user => user.NormalizedEmail == normalizedValue || user.NormalizedUsername == normalizedValue,
                cancellationToken);
    }

    public Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users
            .Include(user => user.Tenant)
            .FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);
    }

    public Task<Role?> GetRoleByNameAsync(Guid tenantId, string roleName, CancellationToken cancellationToken = default)
    {
        return _dbContext.Roles.FirstOrDefaultAsync(
            role => role.TenantId == tenantId && role.Name == roleName,
            cancellationToken);
    }

    public async Task<IReadOnlyCollection<Permission>> GetAllPermissionsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Permissions
            .OrderBy(permission => permission.Code)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<string>> GetRoleNamesAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserRoles
            .Where(userRole => userRole.UserId == userId && userRole.TenantId == tenantId)
            .Select(userRole => userRole.Role!.Name)
            .Distinct()
            .OrderBy(roleName => roleName)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<string>> GetPermissionCodesAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserRoles
            .Where(userRole => userRole.UserId == userId && userRole.TenantId == tenantId)
            .SelectMany(userRole => userRole.Role!.RolePermissions.Select(rolePermission => rolePermission.Permission!.Code))
            .Distinct()
            .OrderBy(permissionCode => permissionCode)
            .ToArrayAsync(cancellationToken);
    }

    public Task<bool> UserHasPermissionAsync(Guid userId, Guid tenantId, string permissionCode, CancellationToken cancellationToken = default)
    {
        return _dbContext.UserRoles
            .Where(userRole => userRole.UserId == userId && userRole.TenantId == tenantId)
            .SelectMany(userRole => userRole.Role!.RolePermissions)
            .AnyAsync(rolePermission => rolePermission.Permission!.Code == permissionCode, cancellationToken);
    }

    public void AddTenant(Tenant tenant)
    {
        _dbContext.Tenants.Add(tenant);
    }

    public void AddUser(User user)
    {
        _dbContext.Users.Add(user);
    }

    public void AddRole(Role role)
    {
        _dbContext.Roles.Add(role);
    }

    public void AddUserRole(UserRole userRole)
    {
        _dbContext.UserRoles.Add(userRole);
    }

    public void AddRolePermissions(IEnumerable<RolePermission> rolePermissions)
    {
        _dbContext.RolePermissions.AddRange(rolePermissions);
    }

}
