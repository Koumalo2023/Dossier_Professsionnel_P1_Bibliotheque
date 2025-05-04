using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class MakeRefreshTokenFieldsNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
            name: "ReplacedByToken",
            table: "RefreshTokens",
            nullable: true, // Rendre nullable
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ReasonRevoked",
                table: "RefreshTokens",
                nullable: true, // Rendre nullable
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
