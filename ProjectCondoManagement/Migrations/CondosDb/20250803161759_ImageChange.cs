using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectCondoManagement.Migrations.CondosDb
{
    /// <inheritdoc />
    public partial class ImageChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "CondoMembers");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "CondoMembers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "CondoMembers");

            migrationBuilder.AddColumn<Guid>(
                name: "ImageId",
                table: "CondoMembers",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
