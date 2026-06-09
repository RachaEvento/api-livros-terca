using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeuAcervo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCustomFieldKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_custom_field_definitions_TenantId_EntityType_NormalizedKey",
                table: "custom_field_definitions");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "custom_field_definitions");

            migrationBuilder.DropColumn(
                name: "NormalizedKey",
                table: "custom_field_definitions");

            migrationBuilder.CreateIndex(
                name: "IX_custom_field_definitions_TenantId_EntityType_SortOrder",
                table: "custom_field_definitions",
                columns: new[] { "TenantId", "EntityType", "SortOrder" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_custom_field_definitions_TenantId_EntityType_SortOrder",
                table: "custom_field_definitions");

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "custom_field_definitions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedKey",
                table: "custom_field_definitions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_custom_field_definitions_TenantId_EntityType_NormalizedKey",
                table: "custom_field_definitions",
                columns: new[] { "TenantId", "EntityType", "NormalizedKey" },
                unique: true,
                filter: "\"IsDeleted\" = FALSE");
        }
    }
}
