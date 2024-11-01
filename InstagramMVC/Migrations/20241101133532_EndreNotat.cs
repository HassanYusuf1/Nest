using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstagramMVC.Migrations
{
    /// <inheritdoc />
    public partial class EndreNotat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrukerId",
                table: "Notes");

            migrationBuilder.AddColumn<string>(
                name: "username",
                table: "Notes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NoteId",
                table: "Kommentarer",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kommentarer_NoteId",
                table: "Kommentarer",
                column: "NoteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Kommentarer_Notes_NoteId",
                table: "Kommentarer",
                column: "NoteId",
                principalTable: "Notes",
                principalColumn: "NoteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kommentarer_Notes_NoteId",
                table: "Kommentarer");

            migrationBuilder.DropIndex(
                name: "IX_Kommentarer_NoteId",
                table: "Kommentarer");

            migrationBuilder.DropColumn(
                name: "username",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "NoteId",
                table: "Kommentarer");

            migrationBuilder.AddColumn<int>(
                name: "BrukerId",
                table: "Notes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
