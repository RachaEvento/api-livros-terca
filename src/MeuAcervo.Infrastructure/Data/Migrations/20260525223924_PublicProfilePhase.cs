using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeuAcervo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class PublicProfilePhase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_profiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsPublicProfileEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsWishlistPublic = table.Column<bool>(type: "boolean", nullable: false),
                    IsStatsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    IsRecentActivityPublic = table.Column<bool>(type: "boolean", nullable: false),
                    FavoriteQuoteOrHeadline = table.Column<string>(type: "character varying(280)", maxLength: 280, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_profiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_profiles_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_library_items_TenantId_UserId_FinishedAt",
                table: "user_library_items",
                columns: new[] { "TenantId", "UserId", "FinishedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_user_library_items_TenantId_UserId_StartedAt",
                table: "user_library_items",
                columns: new[] { "TenantId", "UserId", "StartedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_reviews_TenantId_UserId_Visibility_PublishedAtUtc",
                table: "reviews",
                columns: new[] { "TenantId", "UserId", "Visibility", "PublishedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_user_profiles_TenantId_IsPublicProfileEnabled",
                table: "user_profiles",
                columns: new[] { "TenantId", "IsPublicProfileEnabled" });

            migrationBuilder.CreateIndex(
                name: "IX_user_profiles_UserId",
                table: "user_profiles",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_profiles");

            migrationBuilder.DropIndex(
                name: "IX_user_library_items_TenantId_UserId_FinishedAt",
                table: "user_library_items");

            migrationBuilder.DropIndex(
                name: "IX_user_library_items_TenantId_UserId_StartedAt",
                table: "user_library_items");

            migrationBuilder.DropIndex(
                name: "IX_reviews_TenantId_UserId_Visibility_PublishedAtUtc",
                table: "reviews");
        }
    }
}
