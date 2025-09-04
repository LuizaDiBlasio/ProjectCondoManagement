using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectCondoManagement.Migrations.CondosDb
{
    /// <inheritdoc />
    public partial class Occurrence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Occurences_Condominiums_CondominiumId",
                table: "Occurences");

            migrationBuilder.DropColumn(
                name: "Meeting",
                table: "Occurences");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ResolutionDate",
                table: "Occurences",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "CondominiumId",
                table: "Occurences",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsResolved",
                table: "Occurences",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Occurences_Condominiums_CondominiumId",
                table: "Occurences",
                column: "CondominiumId",
                principalTable: "Condominiums",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Occurences_Condominiums_CondominiumId",
                table: "Occurences");

            migrationBuilder.DropColumn(
                name: "IsResolved",
                table: "Occurences");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ResolutionDate",
                table: "Occurences",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CondominiumId",
                table: "Occurences",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Meeting",
                table: "Occurences",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Occurences_Condominiums_CondominiumId",
                table: "Occurences",
                column: "CondominiumId",
                principalTable: "Condominiums",
                principalColumn: "Id");
        }
    }
}
