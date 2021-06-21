using Microsoft.EntityFrameworkCore.Migrations;

namespace Dryer_Sqlite_Persistance.Migrations.Historical
{
    public partial class Added_Ids : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_State_ChamberId_TimestampUtc",
                table: "State");

            migrationBuilder.DropIndex(
                name: "IX_Sensor_ChamberId_TimestampUtc",
                table: "Sensor");

            migrationBuilder.AddPrimaryKey(
                name: "PK_State",
                table: "State",
                columns: new[] { "ChamberId", "TimestampUtc" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sensor",
                table: "Sensor",
                columns: new[] { "ChamberId", "TimestampUtc" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_State",
                table: "State");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sensor",
                table: "Sensor");

            migrationBuilder.CreateIndex(
                name: "IX_State_ChamberId_TimestampUtc",
                table: "State",
                columns: new[] { "ChamberId", "TimestampUtc" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sensor_ChamberId_TimestampUtc",
                table: "Sensor",
                columns: new[] { "ChamberId", "TimestampUtc" },
                unique: true);
        }
    }
}
