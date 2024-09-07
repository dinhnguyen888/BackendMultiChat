using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendMultiChat.Migrations
{
    /// <inheritdoc />
    public partial class ChatMigration5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(

                name: "FileUrl",
                table: "FileSaveInServers",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileUrl",
                table: "FileSaveInServers");
        }
    }
}
