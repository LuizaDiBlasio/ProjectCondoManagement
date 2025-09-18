using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectCondoManagement.Migrations.CondosDb
{
    /// <inheritdoc />
    public partial class VoteVotingDocument_Removal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Meeting_Documents_ReportId",
                table: "Meeting");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Votes");

            migrationBuilder.DropTable(
                name: "Voting");

            migrationBuilder.DropIndex(
                name: "IX_Meeting_ReportId",
                table: "Meeting");

            migrationBuilder.DropColumn(
                name: "ReportId",
                table: "Meeting");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReportId",
                table: "Meeting",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CondominiumId = table.Column<int>(type: "int", nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataUpload = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_Condominiums_CondominiumId",
                        column: x => x.CondominiumId,
                        principalTable: "Condominiums",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Voting",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Matter = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MeetingId = table.Column<int>(type: "int", nullable: false),
                    Result = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Votes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CondoMemberId = table.Column<int>(type: "int", nullable: false),
                    VotingId = table.Column<int>(type: "int", nullable: true),
                    YesNoVote = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Votes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Votes_CondoMembers_CondoMemberId",
                        column: x => x.CondoMemberId,
                        principalTable: "CondoMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Votes_Voting_VotingId",
                        column: x => x.VotingId,
                        principalTable: "Voting",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Meeting_ReportId",
                table: "Meeting",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CondominiumId",
                table: "Documents",
                column: "CondominiumId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_CondoMemberId",
                table: "Votes",
                column: "CondoMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_Votes_VotingId",
                table: "Votes",
                column: "VotingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Meeting_Documents_ReportId",
                table: "Meeting",
                column: "ReportId",
                principalTable: "Documents",
                principalColumn: "Id");
        }
    }
}
