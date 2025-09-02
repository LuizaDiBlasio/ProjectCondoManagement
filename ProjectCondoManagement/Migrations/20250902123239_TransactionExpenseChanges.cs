using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectCondoManagement.Migrations
{
    /// <inheritdoc />
    public partial class TransactionExpenseChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Expenses_OneTimeExpenseId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_PaymentId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Payments_OneTimeExpenseId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "OneTimeExpenseId",
                table: "Payments");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentId",
                table: "Transactions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Transactions",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentId1",
                table: "Expenses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PaymentId",
                table: "Transactions",
                column: "PaymentId",
                unique: true,
                filter: "[PaymentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_PaymentId1",
                table: "Expenses",
                column: "PaymentId1",
                unique: true,
                filter: "[PaymentId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Payments_PaymentId1",
                table: "Expenses",
                column: "PaymentId1",
                principalTable: "Payments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Payments_PaymentId1",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_PaymentId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_PaymentId1",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "PaymentId1",
                table: "Expenses");

            migrationBuilder.AlterColumn<int>(
                name: "PaymentId",
                table: "Transactions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OneTimeExpenseId",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_PaymentId",
                table: "Transactions",
                column: "PaymentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OneTimeExpenseId",
                table: "Payments",
                column: "OneTimeExpenseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Expenses_OneTimeExpenseId",
                table: "Payments",
                column: "OneTimeExpenseId",
                principalTable: "Expenses",
                principalColumn: "Id");
        }
    }
}
