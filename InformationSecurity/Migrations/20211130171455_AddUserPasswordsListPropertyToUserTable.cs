using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InformationSecurity.Migrations
{
    public partial class AddUserPasswordsListPropertyToUserTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "UserPasswords",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPasswords_UserId",
                table: "UserPasswords",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserPasswords_Users_UserId",
                table: "UserPasswords",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPasswords_Users_UserId",
                table: "UserPasswords");

            migrationBuilder.DropIndex(
                name: "IX_UserPasswords_UserId",
                table: "UserPasswords");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserPasswords");
        }
    }
}
