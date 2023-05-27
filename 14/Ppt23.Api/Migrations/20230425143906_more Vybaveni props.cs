using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ppt23.Api.Migrations
{
    /// <inheritdoc />
    public partial class moreVybaveniprops : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BoughtDateTime",
                table: "Vybavenis",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastRevisionDateTime",
                table: "Vybavenis",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "PriceCzk",
                table: "Vybavenis",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoughtDateTime",
                table: "Vybavenis");

            migrationBuilder.DropColumn(
                name: "LastRevisionDateTime",
                table: "Vybavenis");

            migrationBuilder.DropColumn(
                name: "PriceCzk",
                table: "Vybavenis");
        }
    }
}
