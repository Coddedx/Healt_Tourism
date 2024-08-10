using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Plastic.Migrations
{
    /// <inheritdoc />
    public partial class DoctorFranchiseClinicForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Franchises_FranchiseId",
                table: "Doctors");

            migrationBuilder.AlterColumn<int>(
                name: "FranchiseId",
                table: "Doctors",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "ClinicId",
                table: "Doctors",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_ClinicId",
                table: "Doctors",
                column: "ClinicId");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Clinics_ClinicId",
                table: "Doctors",
                column: "ClinicId",
                principalTable: "Clinics",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Franchises_FranchiseId",
                table: "Doctors",
                column: "FranchiseId",
                principalTable: "Franchises",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Clinics_ClinicId",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_Franchises_FranchiseId",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_ClinicId",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "ClinicId",
                table: "Doctors");

            migrationBuilder.AlterColumn<int>(
                name: "FranchiseId",
                table: "Doctors",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_Franchises_FranchiseId",
                table: "Doctors",
                column: "FranchiseId",
                principalTable: "Franchises",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
