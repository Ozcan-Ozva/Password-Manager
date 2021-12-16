using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InformationSecurity.Migrations
{
    public partial class EditUserPasswordTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPasswords_UploadFiles_UploadFileId",
                table: "UserPasswords");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPasswords_Users_UserId",
                table: "UserPasswords");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "UserPasswords",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UploadFileId",
                table: "UserPasswords",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPasswords_UploadFiles_UploadFileId",
                table: "UserPasswords",
                column: "UploadFileId",
                principalTable: "UploadFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPasswords_Users_UserId",
                table: "UserPasswords",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPasswords_UploadFiles_UploadFileId",
                table: "UserPasswords");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPasswords_Users_UserId",
                table: "UserPasswords");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "UserPasswords",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "UploadFileId",
                table: "UserPasswords",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPasswords_UploadFiles_UploadFileId",
                table: "UserPasswords",
                column: "UploadFileId",
                principalTable: "UploadFiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPasswords_Users_UserId",
                table: "UserPasswords",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
