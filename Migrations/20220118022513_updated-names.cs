using Microsoft.EntityFrameworkCore.Migrations;

namespace Pweb_2021.Migrations
{
    public partial class updatednames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "comentario",
                table: "Reservas",
                newName: "Comentario");

            migrationBuilder.RenameColumn(
                name: "avaliacao",
                table: "Reservas",
                newName: "Avaliacao");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Comentario",
                table: "Reservas",
                newName: "comentario");

            migrationBuilder.RenameColumn(
                name: "Avaliacao",
                table: "Reservas",
                newName: "avaliacao");
        }
    }
}
