using Microsoft.EntityFrameworkCore.Migrations;

namespace Dryer_Sqlite_Persistance.Migrations.Historical
{
    public partial class PersistListeningState : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsListening",
                table: "State",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsListening",
                table: "State");
        }
    }
}
