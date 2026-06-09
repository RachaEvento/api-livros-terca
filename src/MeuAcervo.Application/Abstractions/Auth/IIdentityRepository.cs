using MeuAcervo.Domain.Entities;

namespace MeuAcervo.Application.Abstractions.Auth;

public interface IIdentityRepository
{
    Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken cancellationToken = default);

    Task<bool> UsernameExistsAsync(string normalizedUsername, CancellationToken cancellationToken = default);

    Task<User?> FindByEmailOrUsernameAsync(string normalizedValue, CancellationToken cancellationToken = default);

    Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Role?> GetRoleByNameAsync(Guid tenantId, string roleName, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<Permission>> GetAllPermissionsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<string>> GetRoleNamesAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<string>> GetPermissionCodesAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken = default);

    Task<bool> UserHasPermissionAsync(Guid userId, Guid tenantId, string permissionCode, CancellationToken cancellationToken = default);

    void AddTenant(Tenant tenant);

    void AddUser(User user);

    void AddRole(Role role);

    void AddUserRole(UserRole userRole);

    void AddRolePermissions(IEnumerable<RolePermission> rolePermissions);

}
