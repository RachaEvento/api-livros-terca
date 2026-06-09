using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MeuAcervo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTagsAndCustomFieldRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_library_item_tags");

            migrationBuilder.DropTable(
                name: "tags");

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("584214b8-c3f7-4458-b34b-a2c5d3ca1308"));

            migrationBuilder.DeleteData(
                table: "permissions",
                keyColumn: "Id",
                keyValue: new Guid("8a0fbd47-0cfe-48a0-8d35-a76215ce2807"));

            migrationBuilder.DropColumn(
                name: "IsRequired",
                table: "custom_field_definitions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRequired",
                table: "custom_field_definitions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Color = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "user_library_item_tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserLibraryItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_library_item_tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_library_item_tags_tags_TagId",
                        column: x => x.TagId,
                        principalTable: "tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_user_library_item_tags_user_library_items_UserLibraryItemId",
                        column: x => x.UserLibraryItemId,
                        principalTable: "user_library_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "permissions",
                columns: new[] { "Id", "Code", "CreatedAtUtc", "Description", "UpdatedAtUtc" },
                values: new object[,]
                {
                    { new Guid("584214b8-c3f7-4458-b34b-a2c5d3ca1308"), "tags.write", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Permite alterar tags.", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("8a0fbd47-0cfe-48a0-8d35-a76215ce2807"), "tags.read", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc), "Permite visualizar tags.", new DateTime(2026, 5, 25, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_tags_TenantId_Slug",
                table: "tags",
                columns: new[] { "TenantId", "Slug" },
                unique: true,
                filter: "\"IsDeleted\" = FALSE");

            migrationBuilder.CreateIndex(
                name: "IX_user_library_item_tags_TagId",
                table: "user_library_item_tags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_user_library_item_tags_TenantId_UserLibraryItemId_TagId",
                table: "user_library_item_tags",
                columns: new[] { "TenantId", "UserLibraryItemId", "TagId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_library_item_tags_UserLibraryItemId",
                table: "user_library_item_tags",
                column: "UserLibraryItemId");
        }
    }
}
