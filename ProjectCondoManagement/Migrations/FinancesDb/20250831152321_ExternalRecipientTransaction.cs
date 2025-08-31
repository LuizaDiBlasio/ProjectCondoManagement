using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectCondoManagement.Migrations.FinancesDb
{
    /// <inheritdoc />
    public partial class ExternalRecipientTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalRecipientBankAccount",
                table: "Payments");

            migrationBuilder.AddColumn<string>(
                name: "ExternalRecipientBankAccount",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalRecipientBankAccount",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalRecipientBankAccount",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ExternalRecipientBankAccount",
                table: "Invoices");

            migrationBuilder.AddColumn<string>(
                name: "ExternalRecipientBankAccount",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
