using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendMultiChat.Migrations
{
    /// <inheritdoc />
    public partial class AddLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoLists_Accounts_AccountId",
                table: "TodoLists");

            migrationBuilder.DropIndex(
                name: "IX_TodoLists_AccountId",
                table: "TodoLists");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "TodoLists");

            migrationBuilder.CreateIndex(
                name: "IX_TodoLists_UserId",
                table: "TodoLists",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoLists_Accounts_UserId",
                table: "TodoLists",
                column: "UserId",
                principalTable: "Accounts",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TodoLists_Accounts_UserId",
                table: "TodoLists");

            migrationBuilder.DropIndex(
                name: "IX_TodoLists_UserId",
                table: "TodoLists");

            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "TodoLists",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TodoLists_AccountId",
                table: "TodoLists",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_TodoLists_Accounts_AccountId",
                table: "TodoLists",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "AccountId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
