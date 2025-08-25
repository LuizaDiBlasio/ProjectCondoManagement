using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectCondoManagement.Migrations.FinancesDb
{
    /// <inheritdoc />
    public partial class UpdateFinances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_FinancialAccounts_FinancialAccountId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Payments_Id",
                table: "Invoices");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Payments",
                newName: "UserEmail");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Invoices",
                newName: "UserEmail");

            migrationBuilder.AddColumn<string>(
                name: "AssociatedBankAccount",
                table: "FinancialAccounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "FinancialAccounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CardNumber",
                table: "FinancialAccounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "FinancialAccounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CondominiumId",
                table: "Expenses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_FinancialAccounts_FinancialAccountId",
                table: "Invoices",
                column: "FinancialAccountId",
                principalTable: "FinancialAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Payments_Id",
                table: "Invoices",
                column: "Id",
                principalTable: "Payments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_FinancialAccounts_FinancialAccountId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Payments_Id",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "AssociatedBankAccount",
                table: "FinancialAccounts");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "FinancialAccounts");

            migrationBuilder.DropColumn(
                name: "CardNumber",
                table: "FinancialAccounts");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "FinancialAccounts");

            migrationBuilder.DropColumn(
                name: "CondominiumId",
                table: "Expenses");

            migrationBuilder.RenameColumn(
                name: "UserEmail",
                table: "Payments",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "UserEmail",
                table: "Invoices",
                newName: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_FinancialAccounts_FinancialAccountId",
                table: "Invoices",
                column: "FinancialAccountId",
                principalTable: "FinancialAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Payments_Id",
                table: "Invoices",
                column: "Id",
                principalTable: "Payments",
                principalColumn: "Id");
        }
    }
}
