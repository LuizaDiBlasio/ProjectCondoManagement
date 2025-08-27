using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectCondoManagement.Migrations.CondosDb
{
    /// <inheritdoc />
    public partial class RemoveCondoFromMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CondoMembers_Condominiums_CondominiumId",
                table: "CondoMembers");

            migrationBuilder.DropIndex(
                name: "IX_CondoMembers_CondominiumId",
                table: "CondoMembers");

            migrationBuilder.DropColumn(
                name: "CondominiumId",
                table: "CondoMembers");

            migrationBuilder.DropColumn(
                name: "ConduminiumId",
                table: "CondoMembers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CondominiumId",
                table: "CondoMembers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConduminiumId",
                table: "CondoMembers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CondoMembers_CondominiumId",
                table: "CondoMembers",
                column: "CondominiumId");

            migrationBuilder.AddForeignKey(
                name: "FK_CondoMembers_Condominiums_CondominiumId",
                table: "CondoMembers",
                column: "CondominiumId",
                principalTable: "Condominiums",
                principalColumn: "Id");
        }
    }
}
