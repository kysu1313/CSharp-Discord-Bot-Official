using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ClassLibrary.Migrations
{
    public partial class ReminderUpd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "createdById",
                table: "ReminderModels",
                type: "decimal(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "createdInServerId",
                table: "ReminderModels",
                type: "decimal(20,0)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "reminderId",
                table: "ReminderModels",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "createdById",
                table: "ReminderModels");

            migrationBuilder.DropColumn(
                name: "createdInServerId",
                table: "ReminderModels");

            migrationBuilder.DropColumn(
                name: "reminderId",
                table: "ReminderModels");
        }
    }
}
