using Microsoft.EntityFrameworkCore.Migrations;

namespace ClassLibrary.Migrations
{
    public partial class AddedUserStatModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserStatsModelid",
                table: "StatModels",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserStatModels",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    serverId = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserStatModels", x => x.id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StatModels_UserStatModels_UserStatsModelid",
                table: "StatModels");

            migrationBuilder.DropTable(
                name: "UserStatModels");

            migrationBuilder.DropIndex(
                name: "IX_StatModels_UserStatsModelid",
                table: "StatModels");

            migrationBuilder.DropColumn(
                name: "UserStatsModelid",
                table: "StatModels");
        }
    }
}
