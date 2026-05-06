using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DernekSitesi.Migrations
{
    /// <inheritdoc />
    public partial class HaberResimEkle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResimYolu",
                table: "Haber",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResimYolu",
                table: "Haber");
        }
    }
}
