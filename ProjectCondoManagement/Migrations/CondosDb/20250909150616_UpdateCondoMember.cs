using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectCondoManagement.Migrations.CondosDb
{
    /// <inheritdoc />
    public partial class UpdateCondoMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MeetingType",
                table: "Meeting",
                newName: "IsExtraMeeting");

            migrationBuilder.AddColumn<int>(
                name: "FinancialAccountId",
                table: "CondoMembers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinancialAccountId",
                table: "CondoMembers");

            migrationBuilder.RenameColumn(
                name: "IsExtraMeeting",
                table: "Meeting",
                newName: "MeetingType");
        }
    }
}
