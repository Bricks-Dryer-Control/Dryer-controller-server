using Microsoft.EntityFrameworkCore.Migrations;

namespace Dryer_Sqlite_Persistance.Migrations.Historical
{
    public partial class StateTypeChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualActuator",
                table: "State");

            migrationBuilder.DropColumn(
                name: "Current1",
                table: "State");

            migrationBuilder.RenameColumn(
                name: "workingStatus",
                table: "State",
                newName: "Working");

            migrationBuilder.RenameColumn(
                name: "Setted4",
                table: "State",
                newName: "ThroughFlowSet");

            migrationBuilder.RenameColumn(
                name: "Setted3",
                table: "State",
                newName: "ThroughFlowPosition");

            migrationBuilder.RenameColumn(
                name: "Setted2",
                table: "State",
                newName: "OutFlowSet");

            migrationBuilder.RenameColumn(
                name: "Setted1",
                table: "State",
                newName: "OutFlowPosition");

            migrationBuilder.RenameColumn(
                name: "Current4",
                table: "State",
                newName: "IsAuto");

            migrationBuilder.RenameColumn(
                name: "Current3",
                table: "State",
                newName: "InFlowSet");

            migrationBuilder.RenameColumn(
                name: "Current2",
                table: "State",
                newName: "InFlowPosition");

            migrationBuilder.AddColumn<int>(
                name: "QueuePosition",
                table: "State",
                type: "INTEGER",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QueuePosition",
                table: "State");

            migrationBuilder.RenameColumn(
                name: "Working",
                table: "State",
                newName: "workingStatus");

            migrationBuilder.RenameColumn(
                name: "ThroughFlowSet",
                table: "State",
                newName: "Setted4");

            migrationBuilder.RenameColumn(
                name: "ThroughFlowPosition",
                table: "State",
                newName: "Setted3");

            migrationBuilder.RenameColumn(
                name: "OutFlowSet",
                table: "State",
                newName: "Setted2");

            migrationBuilder.RenameColumn(
                name: "OutFlowPosition",
                table: "State",
                newName: "Setted1");

            migrationBuilder.RenameColumn(
                name: "IsAuto",
                table: "State",
                newName: "Current4");

            migrationBuilder.RenameColumn(
                name: "InFlowSet",
                table: "State",
                newName: "Current3");

            migrationBuilder.RenameColumn(
                name: "InFlowPosition",
                table: "State",
                newName: "Current2");

            migrationBuilder.AddColumn<int>(
                name: "ActualActuator",
                table: "State",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Current1",
                table: "State",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
