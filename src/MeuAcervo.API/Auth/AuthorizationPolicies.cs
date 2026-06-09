using MeuAcervo.Domain.Constants;

namespace MeuAcervo.API.Auth;

public static class AuthorizationPolicies
{
    public const string CanReadLibrary = nameof(CanReadLibrary);
    public const string CanWriteLibrary = nameof(CanWriteLibrary);
    public const string CanReadTags = nameof(CanReadTags);
    public const string CanManageTags = nameof(CanManageTags);
    public const string CanReadLoans = nameof(CanReadLoans);
    public const string CanManageLoans = nameof(CanManageLoans);
    public const string CanReadCustomFields = nameof(CanReadCustomFields);
    public const string CanManageCustomFields = nameof(CanManageCustomFields);
    public const string CanReadReviews = nameof(CanReadReviews);
    public const string CanWriteReviews = nameof(CanWriteReviews);
    public const string CanManageRoles = nameof(CanManageRoles);

    public static IReadOnlyDictionary<string, string> PermissionMap { get; } = new Dictionary<string, string>
    {
        [CanReadLibrary] = PermissionCatalog.LibraryRead.Code,
        [CanWriteLibrary] = PermissionCatalog.LibraryWrite.Code,
        [CanReadTags] = PermissionCatalog.TagsRead.Code,
        [CanManageTags] = PermissionCatalog.TagsWrite.Code,
        [CanReadLoans] = PermissionCatalog.LoansRead.Code,
        [CanManageLoans] = PermissionCatalog.LoansWrite.Code,
        [CanReadCustomFields] = PermissionCatalog.CustomFieldsRead.Code,
        [CanManageCustomFields] = PermissionCatalog.CustomFieldsWrite.Code,
        [CanReadReviews] = PermissionCatalog.ReviewsRead.Code,
        [CanWriteReviews] = PermissionCatalog.ReviewsWrite.Code,
        [CanManageRoles] = PermissionCatalog.AdminRolesManage.Code
    };
}
