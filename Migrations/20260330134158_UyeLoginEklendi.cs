using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DernekSitesi.Migrations
{
    /// <inheritdoc />
    public partial class UyeLoginEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Uyeler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Sifre",
                table: "Uyeler",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Uyeler");

            migrationBuilder.DropColumn(
                name: "Sifre",
                table: "Uyeler");
        }
    }
}
