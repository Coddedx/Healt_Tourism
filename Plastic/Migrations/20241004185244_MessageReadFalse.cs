using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plastic.Migrations
{
    /// <inheritdoc />
    public partial class MessageReadFalse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<bool>(
                name: "Read",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Read",
                table: "Messages");
        }
    }
}
