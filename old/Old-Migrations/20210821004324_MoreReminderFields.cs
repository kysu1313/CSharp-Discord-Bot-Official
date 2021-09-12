using Microsoft.EntityFrameworkCore.Migrations;

namespace ClassLibrary.Migrations
{
    public partial class MoreReminderFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "currentRepeatNumber",
                table: "ReminderModels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "endDate",
                table: "ReminderModels",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "hasExecuted",
                table: "ReminderModels",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "numberOfRepeats",
                table: "ReminderModels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "shouldRepeat",
                table: "ReminderModels",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "currentRepeatNumber",
                table: "ReminderModels");

            migrationBuilder.DropColumn(
                name: "endDate",
                table: "ReminderModels");

            migrationBuilder.DropColumn(
                name: "hasExecuted",
                table: "ReminderModels");

            migrationBuilder.DropColumn(
                name: "numberOfRepeats",
                table: "ReminderModels");

            migrationBuilder.DropColumn(
                name: "shouldRepeat",
                table: "ReminderModels");
        }
    }
}
