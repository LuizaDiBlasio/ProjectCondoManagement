using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectCondoManagement.Migrations.CondosDb
{
    /// <inheritdoc />
    public partial class AddDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentUrl",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CondoName",
                table: "Condominiums",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "FinancialAccountId",
                table: "Condominiums",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CondominiumId",
                table: "CondoMembers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UnitId",
                table: "CondoMembers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CondoMembers_CondominiumId",
                table: "CondoMembers",
                column: "CondominiumId");

            migrationBuilder.CreateIndex(
                name: "IX_CondoMembers_UnitId",
                table: "CondoMembers",
                column: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_CondoMembers_Condominiums_CondominiumId",
                table: "CondoMembers",
                column: "CondominiumId",
                principalTable: "Condominiums",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CondoMembers_Units_UnitId",
                table: "CondoMembers",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CondoMembers_Condominiums_CondominiumId",
                table: "CondoMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_CondoMembers_Units_UnitId",
                table: "CondoMembers");

            migrationBuilder.DropIndex(
                name: "IX_CondoMembers_CondominiumId",
                table: "CondoMembers");

            migrationBuilder.DropIndex(
                name: "IX_CondoMembers_UnitId",
                table: "CondoMembers");

            migrationBuilder.DropColumn(
                name: "DocumentUrl",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "CondoName",
                table: "Condominiums");

            migrationBuilder.DropColumn(
                name: "FinancialAccountId",
                table: "Condominiums");

            migrationBuilder.DropColumn(
                name: "CondominiumId",
                table: "CondoMembers");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "CondoMembers");
        }
    }
}
