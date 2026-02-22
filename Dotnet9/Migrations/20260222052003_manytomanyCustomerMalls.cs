using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dotnet9.Migrations
{
    /// <inheritdoc />
    public partial class manytomanyCustomerMalls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerMalls",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    MallId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerMalls", x => new { x.CustomerId, x.MallId });
                    table.ForeignKey(
                        name: "FK_CustomerMalls_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerMalls_Malls_MallId",
                        column: x => x.MallId,
                        principalTable: "Malls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerMalls_MallId",
                table: "CustomerMalls",
                column: "MallId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerMalls");
        }
    }
}
