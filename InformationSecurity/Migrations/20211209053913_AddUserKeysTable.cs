using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InformationSecurity.Migrations
{
    public partial class AddUserKeysTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UploadFiles_UserPasswords_UserPasswordId",
                table: "UploadFiles");

            migrationBuilder.AddColumn<Guid>(
                name: "UserKeyId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserPasswordId",
                table: "UploadFiles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "UserKeys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrivateKey = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    PublicKey = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserKeys", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserKeyId",
                table: "Users",
                column: "UserKeyId");

            migrationBuilder.AddForeignKey(
                name: "FK_UploadFiles_UserPasswords_UserPasswordId",
                table: "UploadFiles",
                column: "UserPasswordId",
                principalTable: "UserPasswords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserKeys_UserKeyId",
                table: "Users",
                column: "UserKeyId",
                principalTable: "UserKeys",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UploadFiles_UserPasswords_UserPasswordId",
                table: "UploadFiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserKeys_UserKeyId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "UserKeys");

            migrationBuilder.DropIndex(
                name: "IX_Users_UserKeyId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserKeyId",
                table: "Users");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserPasswordId",
                table: "UploadFiles",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_UploadFiles_UserPasswords_UserPasswordId",
                table: "UploadFiles",
                column: "UserPasswordId",
                principalTable: "UserPasswords",
                principalColumn: "Id");
        }
    }
}
