using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InformationSecurity.Migrations
{
    public partial class UpdateUploadFileTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "UploadFiles");

            migrationBuilder.DropColumn(
                name: "DataFile",
                table: "UploadFiles");

            migrationBuilder.DropColumn(
                name: "FileType",
                table: "UploadFiles");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "UploadFiles",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "UploadFiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "UploadFiles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<byte[]>(
                name: "DataFile",
                table: "UploadFiles",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileType",
                table: "UploadFiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
