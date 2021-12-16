using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InformationSecurity.Migrations
{
    public partial class AddUserForigenKeyToUserKeyTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserKeys_UserKeyId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_UserKeyId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserKeyId",
                table: "Users");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "UserKeys",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_UserKeys_UserId",
                table: "UserKeys",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserKeys_Users_UserId",
                table: "UserKeys",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserKeys_Users_UserId",
                table: "UserKeys");

            migrationBuilder.DropIndex(
                name: "IX_UserKeys_UserId",
                table: "UserKeys");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserKeys");

            migrationBuilder.AddColumn<Guid>(
                name: "UserKeyId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserKeyId",
                table: "Users",
                column: "UserKeyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserKeys_UserKeyId",
                table: "Users",
                column: "UserKeyId",
                principalTable: "UserKeys",
                principalColumn: "Id");
        }
    }
}
