using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectCondoManagement.Migrations.FinancesDb
{
    /// <inheritdoc />
    public partial class PaymentAddTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_FinancialAccounts_FinancialAccountId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_FinancialAccountId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "Invoices");

            migrationBuilder.RenameColumn(
                name: "CondominiumId",
                table: "Transactions",
                newName: "PaymentId");

            migrationBuilder.RenameColumn(
                name: "FinancialAccountId",
                table: "Invoices",
                newName: "PaymentId");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "Invoices",
                newName: "PayerFinancialAccountId");

            migrationBuilder.RenameColumn(
                name: "InitialDeposit",
                table: "FinancialAccounts",
                newName: "Deposit");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "CondominiumId",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "InvoiceId",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OneTimeExpenseId",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PayerFinancialAccountId",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TransactionId",
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

            migrationBuilder.AddColumn<int>(
                name: "PayerAccountId",
                table: "Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "FinancialAccounts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OneTimeExpenseId",
                table: "Payments",
                column: "OneTimeExpenseId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_BeneficiaryFinancialAccountId",
                table: "Invoices",
                column: "BeneficiaryFinancialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_PayerFinancialAccountId",
                table: "Invoices",
                column: "PayerFinancialAccountId");

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
                name: "FK_Payments_Expenses_OneTimeExpenseId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_OneTimeExpenseId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_BeneficiaryFinancialAccountId",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_PayerFinancialAccountId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CondominiumId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "OneTimeExpenseId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PayerFinancialAccountId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "BeneficiaryAccountId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "BeneficiaryFinancialAccountId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "PayerAccountId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "FinancialAccounts");

            migrationBuilder.RenameColumn(
                name: "PaymentId",
                table: "Transactions",
                newName: "CondominiumId");

            migrationBuilder.RenameColumn(
                name: "PaymentId",
                table: "Invoices",
                newName: "FinancialAccountId");

            migrationBuilder.RenameColumn(
                name: "PayerFinancialAccountId",
                table: "Invoices",
                newName: "AccountId");

            migrationBuilder.RenameColumn(
                name: "Deposit",
                table: "FinancialAccounts",
                newName: "InitialDeposit");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_FinancialAccountId",
                table: "Invoices",
                column: "FinancialAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_FinancialAccounts_FinancialAccountId",
                table: "Invoices",
                column: "FinancialAccountId",
                principalTable: "FinancialAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
