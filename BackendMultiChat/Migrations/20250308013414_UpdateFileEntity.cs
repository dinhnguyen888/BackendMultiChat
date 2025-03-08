using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendMultiChat.Migrations
{
    /// <inheritdoc />
    public partial class UpdateFileEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupMembers_Rooms_ConversationId",
                table: "GroupMembers");

            migrationBuilder.DropTable(
                name: "FileSaveInServers");

            migrationBuilder.RenameColumn(
                name: "ConversationId",
                table: "GroupMembers",
                newName: "RoomId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupMembers_ConversationId",
                table: "GroupMembers",
                newName: "IX_GroupMembers_RoomId");

            migrationBuilder.CreateTable(
                name: "FileStorages",
                columns: table => new
                {
                    FileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileStorages", x => x.FileId);
                    table.ForeignKey(
                        name: "FK_FileStorages_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileStorages_RoomId",
                table: "FileStorages",
                column: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMembers_Rooms_RoomId",
                table: "GroupMembers",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupMembers_Rooms_RoomId",
                table: "GroupMembers");

            migrationBuilder.DropTable(
                name: "FileStorages");

            migrationBuilder.RenameColumn(
                name: "RoomId",
                table: "GroupMembers",
                newName: "ConversationId");

            migrationBuilder.RenameIndex(
                name: "IX_GroupMembers_RoomId",
                table: "GroupMembers",
                newName: "IX_GroupMembers_ConversationId");

            migrationBuilder.CreateTable(
                name: "FileSaveInServers",
                columns: table => new
                {
                    FileId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConversationID = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileSaveInServers", x => x.FileId);
                    table.ForeignKey(
                        name: "FK_FileSaveInServers_Rooms_ConversationID",
                        column: x => x.ConversationID,
                        principalTable: "Rooms",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileSaveInServers_ConversationID",
                table: "FileSaveInServers",
                column: "ConversationID");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMembers_Rooms_ConversationId",
                table: "GroupMembers",
                column: "ConversationId",
                principalTable: "Rooms",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
