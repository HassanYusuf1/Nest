using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstagramMVC.Migrations
{
    /// <inheritdoc />
    public partial class translate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Kommentarer");

            migrationBuilder.DropTable(
                name: "Bilder");

            migrationBuilder.RenameColumn(
                name: "Tittel",
                table: "Notes",
                newName: "UploadDate");

            migrationBuilder.RenameColumn(
                name: "OpprettetDato",
                table: "Notes",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Innhold",
                table: "Notes",
                newName: "Content");

            migrationBuilder.CreateTable(
                name: "Pictures",
                columns: table => new
                {
                    PictureId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PictureUrl = table.Column<string>(type: "TEXT", nullable: true),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    UploadDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pictures", x => x.PictureId);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    CommentId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PictureId = table.Column<int>(type: "INTEGER", nullable: true),
                    NoteId = table.Column<int>(type: "INTEGER", nullable: true),
                    CommentDescription = table.Column<string>(type: "TEXT", nullable: true),
                    CommentTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.CommentId);
                    table.ForeignKey(
                        name: "FK_Comments_Notes_NoteId",
                        column: x => x.NoteId,
                        principalTable: "Notes",
                        principalColumn: "NoteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Pictures_PictureId",
                        column: x => x.PictureId,
                        principalTable: "Pictures",
                        principalColumn: "PictureId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_NoteId",
                table: "Comments",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PictureId",
                table: "Comments",
                column: "PictureId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Pictures");

            migrationBuilder.RenameColumn(
                name: "UploadDate",
                table: "Notes",
                newName: "Tittel");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Notes",
                newName: "OpprettetDato");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Notes",
                newName: "Innhold");

            migrationBuilder.CreateTable(
                name: "Bilder",
                columns: table => new
                {
                    BildeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Beskrivelse = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    BildeUrl = table.Column<string>(type: "TEXT", nullable: true),
                    OpprettetDato = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Tittel = table.Column<string>(type: "TEXT", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bilder", x => x.BildeId);
                });

            migrationBuilder.CreateTable(
                name: "Kommentarer",
                columns: table => new
                {
                    KommentarId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BildeId = table.Column<int>(type: "INTEGER", nullable: true),
                    NoteId = table.Column<int>(type: "INTEGER", nullable: true),
                    KommentarBeskrivelse = table.Column<string>(type: "TEXT", nullable: true),
                    KommentarTid = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_Kommentarer_Notes_NoteId",
                        column: x => x.NoteId,
                        principalTable: "Notes",
                        principalColumn: "NoteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Kommentarer_BildeId",
                table: "Kommentarer",
                column: "BildeId");

            migrationBuilder.CreateIndex(
                name: "IX_Kommentarer_NoteId",
                table: "Kommentarer",
                column: "NoteId");
        }
    }
}
