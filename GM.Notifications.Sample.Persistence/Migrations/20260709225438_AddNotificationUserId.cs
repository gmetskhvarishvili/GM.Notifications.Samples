using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GM.Notifications.Sample.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "whatsapp",
                table: "WhatsAppNotifications",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "sms",
                table: "SmsNotifications",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "slack",
                table: "SlackNotifications",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "push",
                table: "PushNotifications",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "email",
                table: "EmailNotifications",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "whatsapp",
                table: "WhatsAppNotifications");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "sms",
                table: "SmsNotifications");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "slack",
                table: "SlackNotifications");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "push",
                table: "PushNotifications");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "email",
                table: "EmailNotifications");
        }
    }
}
