using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectCondoManagement.Migrations.FinancesDb
{
    /// <inheritdoc />
    public partial class AddBranchChangesOnFinances : Migration
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

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "FinancialAccounts");

            migrationBuilder.RenameColumn(
                name: "FinancialAccountId",
                table: "Invoices",
                newName: "PayerFinancialAccountId");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "Invoices",
                newName: "PayerAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Invoices_FinancialAccountId",
                table: "Invoices",
                newName: "IX_Invoices_PayerFinancialAccountId");

            migrationBuilder.RenameColumn(
                name: "Assets",
                table: "FinancialAccounts",
                newName: "InitialDeposit");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CondominiumId",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OneTimeExpenseId",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BeneficiaryAccountId",
                table: "Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BeneficiaryFinancialAccountId",
                table: "Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OneTimeExpenseId",
                table: "Payments",
                column: "OneTimeExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_BeneficiaryFinancialAccountId",
                table: "Invoices",
                column: "BeneficiaryFinancialAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_FinancialAccounts_BeneficiaryFinancialAccountId",
                table: "Invoices",
                column: "BeneficiaryFinancialAccountId",
                principalTable: "FinancialAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_FinancialAccounts_PayerFinancialAccountId",
                table: "Invoices",
                column: "PayerFinancialAccountId",
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

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Expenses_OneTimeExpenseId",
                table: "Payments",
                column: "OneTimeExpenseId",
                principalTable: "Expenses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_FinancialAccounts_BeneficiaryFinancialAccountId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_FinancialAccounts_PayerFinancialAccountId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Payments_Id",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Expenses_OneTimeExpenseId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_OneTimeExpenseId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_BeneficiaryFinancialAccountId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CondominiumId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "OneTimeExpenseId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "BeneficiaryAccountId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "BeneficiaryFinancialAccountId",
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
                name: "PayerFinancialAccountId",
                table: "Invoices",
                newName: "FinancialAccountId");

            migrationBuilder.RenameColumn(
                name: "PayerAccountId",
                table: "Invoices",
                newName: "AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Invoices_PayerFinancialAccountId",
                table: "Invoices",
                newName: "IX_Invoices_FinancialAccountId");

            migrationBuilder.RenameColumn(
                name: "InitialDeposit",
                table: "FinancialAccounts",
                newName: "Assets");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "FinancialAccounts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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
