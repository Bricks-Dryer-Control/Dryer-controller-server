using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dryer_Sqlite_Persistance.Migrations.Historical
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sensor",
                columns: table => new
                {
                    ChamberId = table.Column<int>(type: "INTEGER", nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Temperature = table.Column<float>(type: "REAL", nullable: false),
                    Humidity = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "State",
                columns: table => new
                {
                    ChamberId = table.Column<int>(type: "INTEGER", nullable: false),
                    TimestampUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ActualActuator = table.Column<int>(type: "INTEGER", nullable: false),
                    workingStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    Current1 = table.Column<int>(type: "INTEGER", nullable: false),
                    Current2 = table.Column<int>(type: "INTEGER", nullable: false),
                    Current3 = table.Column<int>(type: "INTEGER", nullable: false),
                    Current4 = table.Column<int>(type: "INTEGER", nullable: false),
                    Setted1 = table.Column<int>(type: "INTEGER", nullable: false),
                    Setted2 = table.Column<int>(type: "INTEGER", nullable: false),
                    Setted3 = table.Column<int>(type: "INTEGER", nullable: false),
                    Setted4 = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sensor_ChamberId_TimestampUtc",
                table: "Sensor",
                columns: new[] { "ChamberId", "TimestampUtc" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_State_ChamberId_TimestampUtc",
                table: "State",
                columns: new[] { "ChamberId", "TimestampUtc" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sensor");

            migrationBuilder.DropTable(
                name: "State");
        }
    }
}
