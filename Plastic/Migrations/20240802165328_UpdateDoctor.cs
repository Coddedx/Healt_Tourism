using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plastic.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDoctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl2",
                table: "Doctors",
                newName: "ImageUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Doctors",
                newName: "ImageUrl2");
        }
    }
}
