using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dryer_Sqlite_Persistance.Migrations.AutoControl
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Definitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    TimeToSet = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    TemperatureDifference = table.Column<float>(type: "REAL", nullable: false),
                    ControlDifference = table.Column<int>(type: "INTEGER", nullable: false),
                    ControlType = table.Column<int>(type: "INTEGER", nullable: false),
                    Kp = table.Column<float>(type: "REAL", nullable: false),
                    Ki = table.Column<float>(type: "REAL", nullable: false),
                    MinInFlow = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxInFlow = table.Column<int>(type: "INTEGER", nullable: false),
                    MinOutFlow = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxOutFlow = table.Column<int>(type: "INTEGER", nullable: false),
                    Percent = table.Column<float>(type: "REAL", nullable: false),
                    Offset = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Definitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DefinitionId = table.Column<int>(type: "INTEGER", nullable: false),
                    Time = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    Temperature = table.Column<float>(type: "REAL", nullable: false),
                    InFlow = table.Column<int>(type: "INTEGER", nullable: false),
                    OutFlow = table.Column<int>(type: "INTEGER", nullable: false),
                    ThroughFlow = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sets_Definitions_DefinitionId",
                        column: x => x.DefinitionId,
                        principalTable: "Definitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Definitions_Name",
                table: "Definitions",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Sets_DefinitionId_Time",
                table: "Sets",
                columns: new[] { "DefinitionId", "Time" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sets");

            migrationBuilder.DropTable(
                name: "Definitions");
        }
    }
}
