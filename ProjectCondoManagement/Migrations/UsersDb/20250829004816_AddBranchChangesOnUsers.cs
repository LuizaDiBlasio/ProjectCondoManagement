using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectCondoManagement.Migrations.UsersDb
{
    /// <inheritdoc />
    public partial class AddBranchChangesOnUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyCondominiums");

            migrationBuilder.RenameColumn(
                name: "Addres",
                table: "Companies",
                newName: "Address");

            migrationBuilder.AddColumn<string>(
                name: "CompanyAdminId",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CondominiumIds",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FinancialAccountId",
                table: "Companies",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyAdminId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "CondominiumIds",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "FinancialAccountId",
                table: "Companies");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Companies",
                newName: "Addres");

            migrationBuilder.CreateTable(
                name: "CompanyCondominiums",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CondominiumId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyCondominiums", x => new { x.CompanyId, x.CondominiumId });
                    table.ForeignKey(
                        name: "FK_CompanyCondominiums_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
