using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GM.Notifications.Sample.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MoveMessagingTablesToMessagingSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "messaging");

            migrationBuilder.RenameTable(
                name: "inbox_messages",
                schema: "inbox_messages",
                newName: "inbox_messages",
                newSchema: "messaging");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "inbox_messages");

            migrationBuilder.RenameTable(
                name: "inbox_messages",
                schema: "messaging",
                newName: "inbox_messages",
                newSchema: "inbox_messages");
        }
    }
}
