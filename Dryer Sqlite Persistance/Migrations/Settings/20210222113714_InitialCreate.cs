using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dryer_Sqlite_Persistance.Migrations.Settings
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chamber",
                columns: table => new
                {
                    CreationTimeUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    SensorId = table.Column<int>(type: "INTEGER", nullable: true),
                    InFlowActuatorNo = table.Column<int>(type: "INTEGER", nullable: false),
                    OutFlowActuatorNo = table.Column<int>(type: "INTEGER", nullable: false),
                    ThroughFlowActuatorNo = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chamber_Id_CreationTimeUtc",
                table: "Chamber",
                columns: new[] { "Id", "CreationTimeUtc" },
                unique: true);

            migrationBuilder.InsertData(
                table: "Chamber",
                columns: new[] { "Id", "CreationTimeUtc", "SensorId", "InFlowActuatorNo", "OutFlowActuatorNo", "ThroughFlowActuatorNo" },
                values: new object[,]
                {
                    {1, DateTime.UtcNow, 1, 2, 3, 1 },
                    {2, DateTime.UtcNow, 2, 2, 3, 1 },
                    {3, DateTime.UtcNow, 3, 2, 1, 3 },
                    {4, DateTime.UtcNow, 4, 2, 1, 3 },
                    {5, DateTime.UtcNow, 5, 2, 1, 3 },
                    {6, DateTime.UtcNow, 6, 3, 1, 2 },
                    {7, DateTime.UtcNow, 7, 2, 1, 3 },
                    {8, DateTime.UtcNow, 8, 2, 3, 1 },
                    {9, DateTime.UtcNow, 9, 1, 3, 2 },
                    {10, DateTime.UtcNow, 10, 2, 3, 1 },
                    {11, DateTime.UtcNow, 11, 2, 1, 3 },
                    {12, DateTime.UtcNow, 12, 2, 1, 3 },
                    {13, DateTime.UtcNow, 13, 2, 1, 3 },
                    {14, DateTime.UtcNow, 14, 2, 3, 1 },
                    {15, DateTime.UtcNow, 15, 2, 1, 3 },
                    {16, DateTime.UtcNow, 16, 1, 3, 2 },
                    {17, DateTime.UtcNow, 17, 2, 1, 3 },
                    {18, DateTime.UtcNow, 18, 2, 3, 1 },
                    {19, DateTime.UtcNow, 19, 2, 1, 3 },
                    {20, DateTime.UtcNow, 19, 2, 1, 3 },
                    {21, DateTime.UtcNow, 20, 2, 1, 3 },
                    {22, DateTime.UtcNow, 20, 2, 1, 3 },
                    {23, DateTime.UtcNow, 21, 2, 1, 3 },
                    {24, DateTime.UtcNow, 21, 2, 1, 3 },
                    {25, DateTime.UtcNow, 22, 2, 1, 3 },
                }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chamber");
        }
    }
}
