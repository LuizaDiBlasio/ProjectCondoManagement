using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectCondoManagement.Migrations.FinancesDb
{
    /// <inheritdoc />
    public partial class PaymentExternal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_FinancialAccounts_BeneficiaryFinancialAccountId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_FinancialAccounts_PayerFinancialAccountId",
                table: "Invoices");

            migrationBuilder.AlterColumn<int>(
                name: "BeneficiaryAccountId",
                table: "Transactions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ExternalRecipientBankAccount",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PayerFinancialAccountId",
                table: "Invoices",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "PayerAccountId",
                table: "Invoices",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "BeneficiaryFinancialAccountId",
                table: "Invoices",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "BeneficiaryAccountId",
                table: "Invoices",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_FinancialAccounts_BeneficiaryFinancialAccountId",
                table: "Invoices",
                column: "BeneficiaryFinancialAccountId",
                principalTable: "FinancialAccounts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_FinancialAccounts_PayerFinancialAccountId",
                table: "Invoices",
                column: "PayerFinancialAccountId",
                principalTable: "FinancialAccounts",
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

            migrationBuilder.DropColumn(
                name: "ExternalRecipientBankAccount",
                table: "Payments");

            migrationBuilder.AlterColumn<int>(
                name: "BeneficiaryAccountId",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PayerFinancialAccountId",
                table: "Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PayerAccountId",
                table: "Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BeneficiaryFinancialAccountId",
                table: "Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "BeneficiaryAccountId",
                table: "Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
        }
    }
}
