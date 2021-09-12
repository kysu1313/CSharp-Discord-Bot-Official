using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ClassLibrary.Migrations
{
    public partial class AddReminders : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StatModels_UserStatModels_UserStatsModelid",
                table: "StatModels");

            migrationBuilder.DropIndex(
                name: "IX_StatModels_UserStatsModelid",
                table: "StatModels");

            migrationBuilder.DropColumn(
                name: "UserStatsModelid",
                table: "StatModels");

            migrationBuilder.DropColumn(
                name: "maxLevel",
                table: "StatModels");

            migrationBuilder.DropColumn(
                name: "name",
                table: "StatModels");

            migrationBuilder.DropColumn(
                name: "percent",
                table: "StatModels");

            migrationBuilder.RenameColumn(
                name: "value",
                table: "StatModels",
                newName: "experience");

            migrationBuilder.AddColumn<int>(
                name: "statsid",
                table: "UserStatModels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "bank",
                table: "StatModels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "emojiSent",
                table: "StatModels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "luck",
                table: "StatModels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "messages",
                table: "StatModels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "reactionsReceived",
                table: "StatModels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "serverId",
                table: "StatModels",
                type: "decimal(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "userId",
                table: "StatModels",
                type: "decimal(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "userLevel",
                table: "StatModels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "wallet",
                table: "StatModels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ReminderModels",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    additionalInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    timeAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    executionTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReminderModels", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserStatModels_statsid",
                table: "UserStatModels",
                column: "statsid");

            migrationBuilder.AddForeignKey(
                name: "FK_UserStatModels_StatModels_statsid",
                table: "UserStatModels",
                column: "statsid",
                principalTable: "StatModels",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserStatModels_StatModels_statsid",
                table: "UserStatModels");

            migrationBuilder.DropTable(
                name: "ReminderModels");

            migrationBuilder.DropIndex(
                name: "IX_UserStatModels_statsid",
                table: "UserStatModels");

            migrationBuilder.DropColumn(
                name: "statsid",
                table: "UserStatModels");

            migrationBuilder.DropColumn(
                name: "bank",
                table: "StatModels");

            migrationBuilder.DropColumn(
                name: "emojiSent",
                table: "StatModels");

            migrationBuilder.DropColumn(
                name: "luck",
                table: "StatModels");

            migrationBuilder.DropColumn(
                name: "messages",
                table: "StatModels");

            migrationBuilder.DropColumn(
                name: "reactionsReceived",
                table: "StatModels");

            migrationBuilder.DropColumn(
                name: "serverId",
                table: "StatModels");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "StatModels");

            migrationBuilder.DropColumn(
                name: "userLevel",
                table: "StatModels");

            migrationBuilder.DropColumn(
                name: "wallet",
                table: "StatModels");

            migrationBuilder.RenameColumn(
                name: "experience",
                table: "StatModels",
                newName: "value");

            migrationBuilder.AddColumn<int>(
                name: "UserStatsModelid",
                table: "StatModels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "maxLevel",
                table: "StatModels",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "StatModels",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "percent",
                table: "StatModels",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.CreateIndex(
                name: "IX_StatModels_UserStatsModelid",
                table: "StatModels",
                column: "UserStatsModelid");

            migrationBuilder.AddForeignKey(
                name: "FK_StatModels_UserStatModels_UserStatsModelid",
                table: "StatModels",
                column: "UserStatsModelid",
                principalTable: "UserStatModels",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
