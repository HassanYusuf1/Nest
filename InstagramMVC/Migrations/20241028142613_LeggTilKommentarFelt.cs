using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstagramMVC.Migrations
{
    /// <inheritdoc />
    public partial class LeggTilKommentarFelt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kommentarer",
                columns: table => new
                {
                    KommentarId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BildeId = table.Column<int>(type: "INTEGER", nullable: false),
                    KommentarBeskrivelse = table.Column<string>(type: "TEXT", nullable: true),
                    KommentarTid = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kommentarer", x => x.KommentarId);
                    table.ForeignKey(
                        name: "FK_Kommentarer_Bilder_BildeId",
                        column: x => x.BildeId,
                        principalTable: "Bilder",
                        principalColumn: "BildeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Kommentarer_BildeId",
                table: "Kommentarer",
                column: "BildeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Kommentarer");
        }
    }
}
