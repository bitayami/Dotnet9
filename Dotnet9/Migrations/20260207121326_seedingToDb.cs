using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Dotnet9.Migrations
{
    /// <inheritdoc />
    public partial class seedingToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Malls",
                columns: new[] { "Id", "Floors", "Location", "Name" },
                values: new object[,]
                {
                    { 1, 5, "Downtown", "Sunshine Mall" },
                    { 2, 3, "Riverside", "Riverfront Mall" },
                    { 3, 4, "Hillside", "Mountainview Mall" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Malls",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Malls",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Malls",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
