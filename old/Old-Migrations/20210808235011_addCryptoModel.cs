using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ClassLibrary.Migrations
{
    public partial class addCryptoModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CryptoModels",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    coinName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    price1 = table.Column<int>(type: "int", nullable: false),
                    price2 = table.Column<int>(type: "int", nullable: false),
                    price3 = table.Column<int>(type: "int", nullable: false),
                    price4 = table.Column<int>(type: "int", nullable: false),
                    price5 = table.Column<int>(type: "int", nullable: false),
                    dateUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptoModels", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CryptoModels");
        }
    }
}
