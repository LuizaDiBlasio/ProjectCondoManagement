using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectCondoManagement.Migrations.CondosDb
{
    /// <inheritdoc />
    public partial class CondominiumAndUnitAreNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CondoMembers_Condominiums_CondominiumId",
                table: "CondoMembers");

            migrationBuilder.AlterColumn<string>(
                name: "ConduminiumId",
                table: "CondoMembers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "CondominiumId",
                table: "CondoMembers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_CondoMembers_Condominiums_CondominiumId",
                table: "CondoMembers",
                column: "CondominiumId",
                principalTable: "Condominiums",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CondoMembers_Condominiums_CondominiumId",
                table: "CondoMembers");

            migrationBuilder.AlterColumn<string>(
                name: "ConduminiumId",
                table: "CondoMembers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CondominiumId",
                table: "CondoMembers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CondoMembers_Condominiums_CondominiumId",
                table: "CondoMembers",
                column: "CondominiumId",
                principalTable: "Condominiums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
