using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstagramMVC.Migrations
{
    /// <inheritdoc />
    public partial class EndreKommentarContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kommentarer_Notes_NoteId",
                table: "Kommentarer");

            migrationBuilder.AlterColumn<int>(
                name: "BildeId",
                table: "Kommentarer",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_Kommentarer_Notes_NoteId",
                table: "Kommentarer",
                column: "NoteId",
                principalTable: "Notes",
                principalColumn: "NoteId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kommentarer_Notes_NoteId",
                table: "Kommentarer");

            migrationBuilder.AlterColumn<int>(
                name: "BildeId",
                table: "Kommentarer",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Kommentarer_Notes_NoteId",
                table: "Kommentarer",
                column: "NoteId",
                principalTable: "Notes",
                principalColumn: "NoteId");
        }
    }
}
