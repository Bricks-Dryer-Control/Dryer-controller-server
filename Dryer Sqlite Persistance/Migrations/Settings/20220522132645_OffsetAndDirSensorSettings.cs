using Microsoft.EntityFrameworkCore.Migrations;

namespace Dryer_Sqlite_Persistance.Migrations.Settings
{
    public partial class OffsetAndDirSensorSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DirSensorAffectOutFlow",
                table: "Chamber",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OutFlowOffset",
                table: "Chamber",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DirSensorAffectOutFlow",
                table: "Chamber");

            migrationBuilder.DropColumn(
                name: "OutFlowOffset",
                table: "Chamber");
        }
    }
}
