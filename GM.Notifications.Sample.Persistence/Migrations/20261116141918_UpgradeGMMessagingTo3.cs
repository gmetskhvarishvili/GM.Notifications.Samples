using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GM.Notifications.Sample.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpgradeGMMessagingTo3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "event_version",
                schema: "messaging",
                table: "inbox_messages",
                type: "integer",
                nullable: false,
                defaultValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "event_version",
                schema: "messaging",
                table: "inbox_messages");
        }
    }
}
