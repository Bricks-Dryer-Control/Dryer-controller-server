using Microsoft.EntityFrameworkCore.Migrations;

namespace Dryer_Sqlite_Persistance.Migrations.Settings
{
    public partial class Added_Ids : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Chamber_Id_CreationTimeUtc",
                table: "Chamber");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Chamber",
                table: "Chamber",
                columns: new[] { "Id", "CreationTimeUtc" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Chamber",
                table: "Chamber");

            migrationBuilder.CreateIndex(
                name: "IX_Chamber_Id_CreationTimeUtc",
                table: "Chamber",
                columns: new[] { "Id", "CreationTimeUtc" },
                unique: true);
        }
    }
}
