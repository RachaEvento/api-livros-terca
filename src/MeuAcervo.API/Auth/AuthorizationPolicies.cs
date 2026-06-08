using MeuAcervo.Domain.Constants;

namespace MeuAcervo.API.Auth;

public static class AuthorizationPolicies
{
    public const string CanReadLibrary = nameof(CanReadLibrary);
    public const string CanWriteLibrary = nameof(CanWriteLibrary);
    public const string CanManageTags = nameof(CanManageTags);
    public const string CanManageLoans = nameof(CanManageLoans);
    public const string CanManageCustomFields = nameof(CanManageCustomFields);
    public const string CanManageRoles = nameof(CanManageRoles);

    public static IReadOnlyDictionary<string, string> PermissionMap { get; } = new Dictionary<string, string>
    {
        [CanReadLibrary] = PermissionCatalog.LibraryRead.Code,
        [CanWriteLibrary] = PermissionCatalog.LibraryWrite.Code,
        [CanManageTags] = PermissionCatalog.TagsWrite.Code,
        [CanManageLoans] = PermissionCatalog.LoansWrite.Code,
        [CanManageCustomFields] = PermissionCatalog.CustomFieldsWrite.Code,
        [CanManageRoles] = PermissionCatalog.AdminRolesManage.Code
    };
}
