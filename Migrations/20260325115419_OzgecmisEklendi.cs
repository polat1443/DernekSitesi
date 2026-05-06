using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DernekSitesi.Migrations
{
    /// <inheritdoc />
    public partial class OzgecmisEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ozgecmis",
                table: "YonetimUyeleri",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ozgecmis",
                table: "YonetimUyeleri");
        }
    }
}
