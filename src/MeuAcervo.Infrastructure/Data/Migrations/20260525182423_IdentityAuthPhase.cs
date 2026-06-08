using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MeuAcervo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class IdentityAuthPhase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_Email",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_Username",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_refresh_tokens_UserId_TokenHash",
                table: "refresh_tokens");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "users",
                type: "character varying(320)",
                maxLength: 320,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUsername",
                table: "users",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "JwtId",
                table: "refresh_tokens",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "Id", "Code", "CreatedAtUtc", "Description", "UpdatedAtUtc" },
                values: new object[,]
                {
                    { new Guid("22a45dbf-bd9c-425a-a5c6-87b9a5664206"), "reviews.write", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Permite criar e editar reviews.", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("2d22b8a9-1c99-4e1f-b5c7-1f897dbcc405"), "reviews.read", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Permite visualizar reviews.", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("3034ee58-f9b1-42f7-b7ff-9222ab7f2613"), "profile.read", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Permite visualizar configurações de perfil.", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("52fbdb1f-8b2d-49a2-aab4-c494fe2bf011"), "custom-fields.read", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Permite visualizar campos customizados.", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("584214b8-c3f7-4458-b34b-a2c5d3ca1308"), "tags.write", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Permite alterar tags.", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("59848cc7-5d14-4c74-9d71-1294bd6cbe03"), "wishlist.read", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Permite visualizar a wishlist.", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("6eb47857-20f4-4891-993a-f89ec0fc2214"), "profile.write", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Permite alterar configurações de perfil.", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("7db8c8d0-fc1f-4df4-9699-0e8dc2305a02"), "library.write", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Permite alterar o acervo.", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("8a0fbd47-0cfe-48a0-8d35-a76215ce2807"), "tags.read", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Permite visualizar tags.", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("91313f53-8bc8-4b44-a1b9-0664c9099e10"), "loans.write", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Permite alterar empréstimos.", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("9f7c36fe-6d36-4a94-8d8f-e1d222b07901"), "library.read", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Permite visualizar o acervo.", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("aa738999-18d5-40d6-9f88-8810cc09c112"), "custom-fields.write", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Permite alterar campos customizados.", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("b71be4c3-46d7-45ea-a1c8-06f319d41215"), "admin.roles.manage", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Permite gerenciar roles e permissões.", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("c80d8454-c9b3-45d9-b794-948b25a9a809"), "loans.read", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Permite visualizar empréstimos.", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("f01be170-c8c5-4a1d-892a-fcf48cf6ea04"), "wishlist.write", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Permite alterar a wishlist.", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_NormalizedEmail",
                table: "users",
                column: "NormalizedEmail",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_NormalizedUsername",
                table: "users",
                column: "NormalizedUsername",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_TokenHash",
                table: "refresh_tokens",
                column: "TokenHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_UserId",
                table: "refresh_tokens",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_users_NormalizedEmail",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_NormalizedUsername",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_refresh_tokens_TokenHash",
                table: "refresh_tokens");

            migrationBuilder.DropIndex(
                name: "IX_refresh_tokens_UserId",
                table: "refresh_tokens");

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("22a45dbf-bd9c-425a-a5c6-87b9a5664206"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("2d22b8a9-1c99-4e1f-b5c7-1f897dbcc405"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("3034ee58-f9b1-42f7-b7ff-9222ab7f2613"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("52fbdb1f-8b2d-49a2-aab4-c494fe2bf011"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("584214b8-c3f7-4458-b34b-a2c5d3ca1308"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("59848cc7-5d14-4c74-9d71-1294bd6cbe03"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("6eb47857-20f4-4891-993a-f89ec0fc2214"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("7db8c8d0-fc1f-4df4-9699-0e8dc2305a02"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("8a0fbd47-0cfe-48a0-8d35-a76215ce2807"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("91313f53-8bc8-4b44-a1b9-0664c9099e10"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("9f7c36fe-6d36-4a94-8d8f-e1d222b07901"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("aa738999-18d5-40d6-9f88-8810cc09c112"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("b71be4c3-46d7-45ea-a1c8-06f319d41215"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("c80d8454-c9b3-45d9-b794-948b25a9a809"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("f01be170-c8c5-4a1d-892a-fcf48cf6ea04"));

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "users");

            migrationBuilder.DropColumn(
                name: "NormalizedUsername",
                table: "users");

            migrationBuilder.DropColumn(
                name: "JwtId",
                table: "refresh_tokens");

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                table: "users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_Username",
                table: "users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_refresh_tokens_UserId_TokenHash",
                table: "refresh_tokens",
                columns: new[] { "UserId", "TokenHash" },
                unique: true);
        }
    }
}
