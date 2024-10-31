using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InstagramMVC.Migrations
{
    /// <inheritdoc />
    public partial class AddUserNameToBilde : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BrukerId",
                table: "Bilder");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Bilder",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Bilder");

            migrationBuilder.AddColumn<int>(
                name: "BrukerId",
                table: "Bilder",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
