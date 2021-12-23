using Microsoft.EntityFrameworkCore.Migrations;

namespace Pweb_2021.Migrations
{
    public partial class correctedSomeStuff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Imoveis");

            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "Imoveis",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Imoveis_IdentityUserId",
                table: "Imoveis",
                column: "IdentityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Imoveis_AspNetUsers_IdentityUserId",
                table: "Imoveis",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Imoveis_AspNetUsers_IdentityUserId",
                table: "Imoveis");

            migrationBuilder.DropIndex(
                name: "IX_Imoveis_IdentityUserId",
                table: "Imoveis");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "Imoveis");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Imoveis",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
