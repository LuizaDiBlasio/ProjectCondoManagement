using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectCondoManagement.Migrations.FinancesDb
{
    /// <inheritdoc />
    public partial class InitCreateFinances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FinancialAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CardNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssociatedBankAccount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpenseType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BeneficiaryAccountId = table.Column<int>(type: "int", nullable: true),
                    ExternalRecipientBankAccount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SelectedBeneficiaryId = table.Column<int>(type: "int", nullable: false),
                    PayerFinancialAccountId = table.Column<int>(type: "int", nullable: false),
                    Payer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Recipient = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CondominiumId = table.Column<int>(type: "int", nullable: false),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    InvoiceId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TransactionId = table.Column<int>(type: "int", nullable: true),
                    MbwayNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditCard = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Detail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CondominiumId = table.Column<int>(type: "int", nullable: false),
                    PaymentId = table.Column<int>(type: "int", nullable: true),
                    ExpenseType = table.Column<int>(type: "int", nullable: false),
                    PaymentId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Expenses_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Expenses_Payments_PaymentId1",
                        column: x => x.PaymentId1,
                        principalTable: "Payments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CondominiumId = table.Column<int>(type: "int", nullable: false),
                    PayerAccountId = table.Column<int>(type: "int", nullable: true),
                    PayerFinancialAccountId = table.Column<int>(type: "int", nullable: true),
                    BeneficiaryAccountId = table.Column<int>(type: "int", nullable: true),
                    BeneficiaryFinancialAccountId = table.Column<int>(type: "int", nullable: true),
                    PaymentId = table.Column<int>(type: "int", nullable: false),
                    ExternalRecipientBankAccount = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_FinancialAccounts_BeneficiaryFinancialAccountId",
                        column: x => x.BeneficiaryFinancialAccountId,
                        principalTable: "FinancialAccounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Invoices_FinancialAccounts_PayerFinancialAccountId",
                        column: x => x.PayerFinancialAccountId,
                        principalTable: "FinancialAccounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Invoices_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateAndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PayerAccountId = table.Column<int>(type: "int", nullable: true),
                    BeneficiaryAccountId = table.Column<int>(type: "int", nullable: true),
                    ExternalRecipientBankAccount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CompanyId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_FinancialAccounts_BeneficiaryAccountId",
                        column: x => x.BeneficiaryAccountId,
                        principalTable: "FinancialAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_FinancialAccounts_PayerAccountId",
                        column: x => x.PayerAccountId,
                        principalTable: "FinancialAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Transactions_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_PaymentId",
                table: "Expenses",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_PaymentId1",
                table: "Expenses",
                column: "PaymentId1",
                unique: true,
                filter: "[PaymentId1] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_BeneficiaryFinancialAccountId",
                table: "Invoices",
                column: "BeneficiaryFinancialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_PayerFinancialAccountId",
                table: "Invoices",
                column: "PayerFinancialAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_PaymentId",
                table: "Invoices",
                column: "PaymentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_BeneficiaryAccountId",
                table: "Transactions",
                column: "BeneficiaryAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PayerAccountId",
                table: "Transactions",
                column: "PayerAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PaymentId",
                table: "Transactions",
                column: "PaymentId",
                unique: true,
                filter: "[PaymentId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "FinancialAccounts");

            migrationBuilder.DropTable(
                name: "Payments");
        }
    }
}
