using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectCondoManagement.Migrations.DataContextCondosMigrations
{
    /// <inheritdoc />
    public partial class InitCreateCondos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CondoMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FinancialAccountId = table.Column<int>(type: "int", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdDocument = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxIdNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CondoMembers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Condominiums",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CondoName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ManagerUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FinancialAccountId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Condominiums", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Meeting",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CondominiumId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateAndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsExtraMeeting = table.Column<bool>(type: "bit", nullable: false),
                    MeetingLink = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meeting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Meeting_Condominiums_CondominiumId",
                        column: x => x.CondominiumId,
                        principalTable: "Condominiums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CondominiumId = table.Column<int>(type: "int", nullable: false),
                    Floor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bedrooms = table.Column<int>(type: "int", nullable: false),
                    Door = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Units_Condominiums_CondominiumId",
                        column: x => x.CondominiumId,
                        principalTable: "Condominiums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CondoMemberMeeting",
                columns: table => new
                {
                    CondoMembersId = table.Column<int>(type: "int", nullable: false),
                    MeetingsAttendedId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CondoMemberMeeting", x => new { x.CondoMembersId, x.MeetingsAttendedId });
                    table.ForeignKey(
                        name: "FK_CondoMemberMeeting_CondoMembers_CondoMembersId",
                        column: x => x.CondoMembersId,
                        principalTable: "CondoMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CondoMemberMeeting_Meeting_MeetingsAttendedId",
                        column: x => x.MeetingsAttendedId,
                        principalTable: "Meeting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Occurences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateAndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResolutionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false),
                    CondominiumId = table.Column<int>(type: "int", nullable: false),
                    MeetingId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Occurences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Occurences_Condominiums_CondominiumId",
                        column: x => x.CondominiumId,
                        principalTable: "Condominiums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Occurences_Meeting_MeetingId",
                        column: x => x.MeetingId,
                        principalTable: "Meeting",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CondoMemberUnit",
                columns: table => new
                {
                    CondoMembersId = table.Column<int>(type: "int", nullable: false),
                    UnitsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CondoMemberUnit", x => new { x.CondoMembersId, x.UnitsId });
                    table.ForeignKey(
                        name: "FK_CondoMemberUnit_CondoMembers_CondoMembersId",
                        column: x => x.CondoMembersId,
                        principalTable: "CondoMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CondoMemberUnit_Units_UnitsId",
                        column: x => x.UnitsId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OccurrenceUnit",
                columns: table => new
                {
                    OccurrencesId = table.Column<int>(type: "int", nullable: false),
                    UnitsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OccurrenceUnit", x => new { x.OccurrencesId, x.UnitsId });
                    table.ForeignKey(
                        name: "FK_OccurrenceUnit_Occurences_OccurrencesId",
                        column: x => x.OccurrencesId,
                        principalTable: "Occurences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OccurrenceUnit_Units_UnitsId",
                        column: x => x.UnitsId,
                        principalTable: "Units",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CondoMemberMeeting_MeetingsAttendedId",
                table: "CondoMemberMeeting",
                column: "MeetingsAttendedId");

            migrationBuilder.CreateIndex(
                name: "IX_CondoMemberUnit_UnitsId",
                table: "CondoMemberUnit",
                column: "UnitsId");

            migrationBuilder.CreateIndex(
                name: "IX_Meeting_CondominiumId",
                table: "Meeting",
                column: "CondominiumId");

            migrationBuilder.CreateIndex(
                name: "IX_Occurences_CondominiumId",
                table: "Occurences",
                column: "CondominiumId");

            migrationBuilder.CreateIndex(
                name: "IX_Occurences_MeetingId",
                table: "Occurences",
                column: "MeetingId");

            migrationBuilder.CreateIndex(
                name: "IX_OccurrenceUnit_UnitsId",
                table: "OccurrenceUnit",
                column: "UnitsId");

            migrationBuilder.CreateIndex(
                name: "IX_Units_CondominiumId",
                table: "Units",
                column: "CondominiumId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CondoMemberMeeting");

            migrationBuilder.DropTable(
                name: "CondoMemberUnit");

            migrationBuilder.DropTable(
                name: "OccurrenceUnit");

            migrationBuilder.DropTable(
                name: "CondoMembers");

            migrationBuilder.DropTable(
                name: "Occurences");

            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropTable(
                name: "Meeting");

            migrationBuilder.DropTable(
                name: "Condominiums");
        }
    }
}
