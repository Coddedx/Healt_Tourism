using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plastic.Migrations
{
    /// <inheritdoc />
    public partial class OperationTableDeleteDoctorId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Operations_Doctors_DoctorId",
                table: "Operations");

            migrationBuilder.DropIndex(
                name: "IX_Operations_DoctorId",
                table: "Operations");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                table: "Operations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DoctorId",
                table: "Operations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Operations_DoctorId",
                table: "Operations",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Operations_Doctors_DoctorId",
                table: "Operations",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id");
        }
    }
}
