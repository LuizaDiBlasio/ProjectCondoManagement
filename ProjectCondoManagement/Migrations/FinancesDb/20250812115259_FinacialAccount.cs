using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectCondoManagement.Migrations.FinancesDb
{
    /// <inheritdoc />
    public partial class FinacialAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Assets",
                table: "FinancialAccounts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "FinancialAccounts");

            migrationBuilder.RenameColumn(
                name: "Balance",
                table: "FinancialAccounts",
                newName: "InitialDeposit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InitialDeposit",
                table: "FinancialAccounts",
                newName: "Balance");

            migrationBuilder.AddColumn<decimal>(
                name: "Assets",
                table: "FinancialAccounts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "FinancialAccounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
