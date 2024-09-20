using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plastic.Migrations
{
    /// <inheritdoc />
    public partial class OperationDoctorIdFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OperationDoctors_OperationDoctors_OperationDoctorId",
                table: "OperationDoctors");

            migrationBuilder.DropIndex(
                name: "IX_OperationDoctors_OperationDoctorId",
                table: "OperationDoctors");

            migrationBuilder.DropColumn(
                name: "OperationDoctorId",
                table: "OperationDoctors");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OperationDoctorId",
                table: "OperationDoctors",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperationDoctors_OperationDoctorId",
                table: "OperationDoctors",
                column: "OperationDoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_OperationDoctors_OperationDoctors_OperationDoctorId",
                table: "OperationDoctors",
                column: "OperationDoctorId",
                principalTable: "OperationDoctors",
                principalColumn: "Id");
        }
    }
}
