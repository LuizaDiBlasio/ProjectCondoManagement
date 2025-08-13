using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectCondoManagement.Migrations.CondosDb
{
    /// <inheritdoc />
    public partial class AddCondoIdCondoName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CondoName",
                table: "Condominiums",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CondominiumId",
                table: "CondoMembers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ConduminiumId",
                table: "CondoMembers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_CondoMembers_CondominiumId",
                table: "CondoMembers",
                column: "CondominiumId");

            migrationBuilder.AddForeignKey(
                name: "FK_CondoMembers_Condominiums_CondominiumId",
                table: "CondoMembers",
                column: "CondominiumId",
                principalTable: "Condominiums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CondoMembers_Condominiums_CondominiumId",
                table: "CondoMembers");

            migrationBuilder.DropIndex(
                name: "IX_CondoMembers_CondominiumId",
                table: "CondoMembers");

            migrationBuilder.DropColumn(
                name: "CondoName",
                table: "Condominiums");

            migrationBuilder.DropColumn(
                name: "CondominiumId",
                table: "CondoMembers");

            migrationBuilder.DropColumn(
                name: "ConduminiumId",
                table: "CondoMembers");
        }
    }
}
