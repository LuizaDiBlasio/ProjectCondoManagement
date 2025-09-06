using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectCondoManagement.Migrations.CondosDb
{
    /// <inheritdoc />
    public partial class MeetingVotingDetach : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meeting_Documents_ReportId",
                table: "Meeting");

            migrationBuilder.DropForeignKey(
                name: "FK_Voting_Meeting_MeetingId",
                table: "Voting");

            migrationBuilder.DropIndex(
                name: "IX_Voting_MeetingId",
                table: "Voting");

            migrationBuilder.AlterColumn<int>(
                name: "ReportId",
                table: "Meeting",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Meeting",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MeetingLink",
                table: "Meeting",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Meeting",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Meeting_Documents_ReportId",
                table: "Meeting",
                column: "ReportId",
                principalTable: "Documents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meeting_Documents_ReportId",
                table: "Meeting");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Meeting");

            migrationBuilder.DropColumn(
                name: "MeetingLink",
                table: "Meeting");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Meeting");

            migrationBuilder.AlterColumn<int>(
                name: "ReportId",
                table: "Meeting",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Voting_MeetingId",
                table: "Voting",
                column: "MeetingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Meeting_Documents_ReportId",
                table: "Meeting",
                column: "ReportId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Voting_Meeting_MeetingId",
                table: "Voting",
                column: "MeetingId",
                principalTable: "Meeting",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
