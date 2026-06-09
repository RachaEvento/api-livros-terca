using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeuAcervo.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyCustomFieldContracts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfigurationJson",
                table: "custom_field_definitions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ConfigurationJson",
                table: "custom_field_definitions",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: true);
        }
    }
}
