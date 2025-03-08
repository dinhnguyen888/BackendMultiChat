using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendMultiChat.Migrations
{
    /// <inheritdoc />
    public partial class ChangeConversationToRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileSaveInServers_Conversations_ConversationID",
                table: "FileSaveInServers");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupMembers_Conversations_ConversationId",
                table: "GroupMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Conversations_ConversationId",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "Conversations");

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    RoomId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.RoomId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_FileSaveInServers_Rooms_ConversationID",
                table: "FileSaveInServers",
                column: "ConversationID",
                principalTable: "Rooms",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMembers_Rooms_ConversationId",
                table: "GroupMembers",
                column: "ConversationId",
                principalTable: "Rooms",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Rooms_ConversationId",
                table: "Messages",
                column: "ConversationId",
                principalTable: "Rooms",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileSaveInServers_Rooms_ConversationID",
                table: "FileSaveInServers");

            migrationBuilder.DropForeignKey(
                name: "FK_GroupMembers_Rooms_ConversationId",
                table: "GroupMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Rooms_ConversationId",
                table: "Messages");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    ConversationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConversationName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.ConversationId);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_FileSaveInServers_Conversations_ConversationID",
                table: "FileSaveInServers",
                column: "ConversationID",
                principalTable: "Conversations",
                principalColumn: "ConversationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupMembers_Conversations_ConversationId",
                table: "GroupMembers",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "ConversationId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Conversations_ConversationId",
                table: "Messages",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "ConversationId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
