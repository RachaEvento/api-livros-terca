namespace MeuAcervo.Domain.Constants;

public sealed record PermissionDefinition(Guid Id, string Code, string Description);

public static class PermissionCatalog
{
    public static readonly PermissionDefinition LibraryRead = new(Guid.Parse("9f7c36fe-6d36-4a94-8d8f-e1d222b07901"), "library.read", "Permite visualizar o acervo.");
    public static readonly PermissionDefinition LibraryWrite = new(Guid.Parse("7db8c8d0-fc1f-4df4-9699-0e8dc2305a02"), "library.write", "Permite alterar o acervo.");
    public static readonly PermissionDefinition WishlistRead = new(Guid.Parse("59848cc7-5d14-4c74-9d71-1294bd6cbe03"), "wishlist.read", "Permite visualizar a wishlist.");
    public static readonly PermissionDefinition WishlistWrite = new(Guid.Parse("f01be170-c8c5-4a1d-892a-fcf48cf6ea04"), "wishlist.write", "Permite alterar a wishlist.");
    public static readonly PermissionDefinition ReviewsRead = new(Guid.Parse("2d22b8a9-1c99-4e1f-b5c7-1f897dbcc405"), "reviews.read", "Permite visualizar reviews.");
    public static readonly PermissionDefinition ReviewsWrite = new(Guid.Parse("22a45dbf-bd9c-425a-a5c6-87b9a5664206"), "reviews.write", "Permite criar e editar reviews.");
    public static readonly PermissionDefinition LoansRead = new(Guid.Parse("c80d8454-c9b3-45d9-b794-948b25a9a809"), "loans.read", "Permite visualizar empr\u00E9stimos.");
    public static readonly PermissionDefinition LoansWrite = new(Guid.Parse("91313f53-8bc8-4b44-a1b9-0664c9099e10"), "loans.write", "Permite alterar empr\u00E9stimos.");
    public static readonly PermissionDefinition CustomFieldsRead = new(Guid.Parse("52fbdb1f-8b2d-49a2-aab4-c494fe2bf011"), "custom-fields.read", "Permite visualizar campos customizados.");
    public static readonly PermissionDefinition CustomFieldsWrite = new(Guid.Parse("aa738999-18d5-40d6-9f88-8810cc09c112"), "custom-fields.write", "Permite alterar campos customizados.");
    public static readonly PermissionDefinition ProfileRead = new(Guid.Parse("3034ee58-f9b1-42f7-b7ff-9222ab7f2613"), "profile.read", "Permite visualizar configura\u00E7\u00F5es de perfil.");
    public static readonly PermissionDefinition ProfileWrite = new(Guid.Parse("6eb47857-20f4-4891-993a-f89ec0fc2214"), "profile.write", "Permite alterar configura\u00E7\u00F5es de perfil.");
    public static readonly PermissionDefinition AdminRolesManage = new(Guid.Parse("b71be4c3-46d7-45ea-a1c8-06f319d41215"), "admin.roles.manage", "Permite gerenciar roles e permiss\u00F5es.");

    public static IReadOnlyCollection<PermissionDefinition> All { get; } =
    [
        LibraryRead,
        LibraryWrite,
        WishlistRead,
        WishlistWrite,
        ReviewsRead,
        ReviewsWrite,
        LoansRead,
        LoansWrite,
        CustomFieldsRead,
        CustomFieldsWrite,
        ProfileRead,
        ProfileWrite,
        AdminRolesManage
    ];
}
