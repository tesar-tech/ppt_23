using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ppt23.Api.Migrations
{
    /// <inheritdoc />
    public partial class renameRevizes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Revize_Vybavenis_VybaveniId",
                table: "Revize");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Revize",
                table: "Revize");

            migrationBuilder.RenameTable(
                name: "Revize",
                newName: "Revizes");

            migrationBuilder.RenameIndex(
                name: "IX_Revize_VybaveniId",
                table: "Revizes",
                newName: "IX_Revizes_VybaveniId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Revizes",
                table: "Revizes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Revizes_Vybavenis_VybaveniId",
                table: "Revizes",
                column: "VybaveniId",
                principalTable: "Vybavenis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Revizes_Vybavenis_VybaveniId",
                table: "Revizes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Revizes",
                table: "Revizes");

            migrationBuilder.RenameTable(
                name: "Revizes",
                newName: "Revize");

            migrationBuilder.RenameIndex(
                name: "IX_Revizes_VybaveniId",
                table: "Revize",
                newName: "IX_Revize_VybaveniId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Revize",
                table: "Revize",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Revize_Vybavenis_VybaveniId",
                table: "Revize",
                column: "VybaveniId",
                principalTable: "Vybavenis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
