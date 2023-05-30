using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ppt23.Api.Migrations
{
    /// <inheritdoc />
    public partial class PracovniciAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PracovnikId",
                table: "Ukons",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Pracovniks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pracovniks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ukons_PracovnikId",
                table: "Ukons",
                column: "PracovnikId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ukons_Pracovniks_PracovnikId",
                table: "Ukons",
                column: "PracovnikId",
                principalTable: "Pracovniks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ukons_Pracovniks_PracovnikId",
                table: "Ukons");

            migrationBuilder.DropTable(
                name: "Pracovniks");

            migrationBuilder.DropIndex(
                name: "IX_Ukons_PracovnikId",
                table: "Ukons");

            migrationBuilder.DropColumn(
                name: "PracovnikId",
                table: "Ukons");
        }
    }
}
