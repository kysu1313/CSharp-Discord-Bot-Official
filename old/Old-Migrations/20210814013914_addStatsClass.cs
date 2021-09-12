using Microsoft.EntityFrameworkCore.Migrations;

namespace ClassLibrary.Migrations
{
    public partial class addStatsClass : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StatModels",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    percent = table.Column<float>(type: "real", nullable: false),
                    maxLevel = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatModels", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "UserDashes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    serverId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    userName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    color = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDashes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "DashItems",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    serverId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    command = table.Column<int>(type: "int", nullable: false),
                    value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    result = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserDashid = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashItems", x => x.id);
                    table.ForeignKey(
                        name: "FK_DashItems_UserDashes_UserDashid",
                        column: x => x.UserDashid,
                        principalTable: "UserDashes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DashItems_UserDashid",
                table: "DashItems",
                column: "UserDashid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DashItems");

            migrationBuilder.DropTable(
                name: "StatModels");

            migrationBuilder.DropTable(
                name: "UserDashes");
        }
    }
}
