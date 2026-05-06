using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DernekSitesi.Migrations
{
    /// <inheritdoc />
    public partial class YonetimKuruluEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "YonetimUyeleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdSoyad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gorev = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ResimYolu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TwitterLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinkedInLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstagramLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Siralama = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YonetimUyeleri", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "YonetimUyeleri");
        }
    }
}
