using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstagramMVC.Migrations
{
    /// <inheritdoc />
    public partial class RenameIdToBildeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Bilder",
                newName: "BildeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BildeId",
                table: "Bilder",
                newName: "Id");
        }
    }
}
