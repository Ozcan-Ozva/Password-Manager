using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InformationSecurity.Migrations
{
    public partial class ModifyUploadFileToCollectionOfUploadFileInUserPasswordTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPasswords_UploadFiles_UploadFileId",
                table: "UserPasswords");

            migrationBuilder.DropIndex(
                name: "IX_UserPasswords_UploadFileId",
                table: "UserPasswords");

            migrationBuilder.DropColumn(
                name: "UploadFileId",
                table: "UserPasswords");

            migrationBuilder.AddColumn<Guid>(
                name: "UserPasswordId",
                table: "UploadFiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UploadFiles_UserPasswordId",
                table: "UploadFiles",
                column: "UserPasswordId");

            migrationBuilder.AddForeignKey(
                name: "FK_UploadFiles_UserPasswords_UserPasswordId",
                table: "UploadFiles",
                column: "UserPasswordId",
                principalTable: "UserPasswords",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UploadFiles_UserPasswords_UserPasswordId",
                table: "UploadFiles");

            migrationBuilder.DropIndex(
                name: "IX_UploadFiles_UserPasswordId",
                table: "UploadFiles");

            migrationBuilder.DropColumn(
                name: "UserPasswordId",
                table: "UploadFiles");

            migrationBuilder.AddColumn<Guid>(
                name: "UploadFileId",
                table: "UserPasswords",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_UserPasswords_UploadFileId",
                table: "UserPasswords",
                column: "UploadFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPasswords_UploadFiles_UploadFileId",
                table: "UserPasswords",
                column: "UploadFileId",
                principalTable: "UploadFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
