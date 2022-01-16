using Microsoft.EntityFrameworkCore.Migrations;

namespace Pweb_2021.Migrations
{
    public partial class added_estado_a_reserva : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte>(
                name: "Estado",
                table: "Reservas",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Reservas");
        }
    }
}
