using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeuAcervo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserLibraryPhase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_library_items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    BookEditionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ShelfType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ReadingStatus = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    AcquisitionFormat = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    OwnershipType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    IsFavorite = table.Column<bool>(type: "boolean", nullable: false),
                    CurrentPage = table.Column<int>(type: "integer", nullable: true),
                    ProgressPercent = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    ReadCount = table.Column<int>(type: "integer", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    FinishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AcquiredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PhysicalLocation = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Condition = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PrivateNotes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_library_items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_library_items_book_editions_BookEditionId",
                        column: x => x.BookEditionId,
                        principalTable: "book_editions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_user_library_items_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "reading_progress_entries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserLibraryItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    PageNumber = table.Column<int>(type: "integer", nullable: true),
                    ProgressPercent = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    RecordedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reading_progress_entries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_reading_progress_entries_user_library_items_UserLibraryItem~",
                        column: x => x.UserLibraryItemId,
                        principalTable: "user_library_items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_reading_progress_entries_TenantId_UserLibraryItemId_Recorde~",
                table: "reading_progress_entries",
                columns: new[] { "TenantId", "UserLibraryItemId", "RecordedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_reading_progress_entries_UserLibraryItemId",
                table: "reading_progress_entries",
                column: "UserLibraryItemId");

            migrationBuilder.CreateIndex(
                name: "IX_user_library_items_BookEditionId",
                table: "user_library_items",
                column: "BookEditionId");

            migrationBuilder.CreateIndex(
                name: "IX_user_library_items_TenantId_UserId_BookEditionId",
                table: "user_library_items",
                columns: new[] { "TenantId", "UserId", "BookEditionId" },
                unique: true,
                filter: "\"IsDeleted\" = FALSE");

            migrationBuilder.CreateIndex(
                name: "IX_user_library_items_TenantId_UserId_IsFavorite",
                table: "user_library_items",
                columns: new[] { "TenantId", "UserId", "IsFavorite" });

            migrationBuilder.CreateIndex(
                name: "IX_user_library_items_TenantId_UserId_ReadingStatus",
                table: "user_library_items",
                columns: new[] { "TenantId", "UserId", "ReadingStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_user_library_items_TenantId_UserId_ShelfType",
                table: "user_library_items",
                columns: new[] { "TenantId", "UserId", "ShelfType" });

            migrationBuilder.CreateIndex(
                name: "IX_user_library_items_UpdatedAtUtc",
                table: "user_library_items",
                column: "UpdatedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_user_library_items_UserId",
                table: "user_library_items",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reading_progress_entries");

            migrationBuilder.DropTable(
                name: "user_library_items");
        }
    }
}
