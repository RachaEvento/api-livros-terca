using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeuAcervo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class CatalogPhase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "authors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Bio = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_authors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "book_works",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CanonicalTitle = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    NormalizedCanonicalTitle = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    OriginalTitle = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: true),
                    FirstPublicationYear = table.Column<int>(type: "integer", nullable: true),
                    PrimaryLanguage = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_book_works", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "publishers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    NormalizedName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_publishers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "book_editions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookWorkId = table.Column<Guid>(type: "uuid", nullable: false),
                    Isbn10 = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Isbn13 = table.Column<string>(type: "character varying(13)", maxLength: 13, nullable: true),
                    Title = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    NormalizedTitle = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Subtitle = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PublisherId = table.Column<Guid>(type: "uuid", nullable: true),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PageCount = table.Column<int>(type: "integer", nullable: true),
                    Language = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    FormatDescriptor = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    CoverImageUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    EditionNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_book_editions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_book_editions_book_works_BookWorkId",
                        column: x => x.BookWorkId,
                        principalTable: "book_works",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_book_editions_publishers_PublisherId",
                        column: x => x.PublisherId,
                        principalTable: "publishers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "book_edition_authors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BookEditionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContributionOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_book_edition_authors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_book_edition_authors_authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_book_edition_authors_book_editions_BookEditionId",
                        column: x => x.BookEditionId,
                        principalTable: "book_editions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "external_book_references",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Provider = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ExternalId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ReferenceType = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    BookWorkId = table.Column<Guid>(type: "uuid", nullable: true),
                    BookEditionId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExternalUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_external_book_references", x => x.Id);
                    table.CheckConstraint("CK_external_book_references_target", "(\"ReferenceType\" = 'Work' AND \"BookWorkId\" IS NOT NULL AND \"BookEditionId\" IS NULL) OR (\"ReferenceType\" = 'Edition' AND \"BookEditionId\" IS NOT NULL AND \"BookWorkId\" IS NULL)");
                    table.ForeignKey(
                        name: "FK_external_book_references_book_editions_BookEditionId",
                        column: x => x.BookEditionId,
                        principalTable: "book_editions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_external_book_references_book_works_BookWorkId",
                        column: x => x.BookWorkId,
                        principalTable: "book_works",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_authors_NormalizedName",
                table: "authors",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_book_edition_authors_AuthorId",
                table: "book_edition_authors",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_book_edition_authors_BookEditionId_AuthorId",
                table: "book_edition_authors",
                columns: new[] { "BookEditionId", "AuthorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_book_edition_authors_BookEditionId_ContributionOrder",
                table: "book_edition_authors",
                columns: new[] { "BookEditionId", "ContributionOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_book_editions_BookWorkId",
                table: "book_editions",
                column: "BookWorkId");

            migrationBuilder.CreateIndex(
                name: "IX_book_editions_Isbn10",
                table: "book_editions",
                column: "Isbn10",
                unique: true,
                filter: "\"Isbn10\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_book_editions_Isbn13",
                table: "book_editions",
                column: "Isbn13",
                unique: true,
                filter: "\"Isbn13\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_book_editions_NormalizedTitle_Language",
                table: "book_editions",
                columns: new[] { "NormalizedTitle", "Language" });

            migrationBuilder.CreateIndex(
                name: "IX_book_editions_PublisherId",
                table: "book_editions",
                column: "PublisherId");

            migrationBuilder.CreateIndex(
                name: "IX_book_works_NormalizedCanonicalTitle",
                table: "book_works",
                column: "NormalizedCanonicalTitle");

            migrationBuilder.CreateIndex(
                name: "IX_book_works_NormalizedCanonicalTitle_PrimaryLanguage",
                table: "book_works",
                columns: new[] { "NormalizedCanonicalTitle", "PrimaryLanguage" });

            migrationBuilder.CreateIndex(
                name: "IX_external_book_references_BookEditionId",
                table: "external_book_references",
                column: "BookEditionId");

            migrationBuilder.CreateIndex(
                name: "IX_external_book_references_BookWorkId",
                table: "external_book_references",
                column: "BookWorkId");

            migrationBuilder.CreateIndex(
                name: "IX_external_book_references_Provider_ExternalId_ReferenceType",
                table: "external_book_references",
                columns: new[] { "Provider", "ExternalId", "ReferenceType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_publishers_NormalizedName",
                table: "publishers",
                column: "NormalizedName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "book_edition_authors");

            migrationBuilder.DropTable(
                name: "external_book_references");

            migrationBuilder.DropTable(
                name: "authors");

            migrationBuilder.DropTable(
                name: "book_editions");

            migrationBuilder.DropTable(
                name: "book_works");

            migrationBuilder.DropTable(
                name: "publishers");
        }
    }
}
