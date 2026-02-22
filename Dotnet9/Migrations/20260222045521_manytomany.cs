using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dotnet9.Migrations
{
    /// <inheritdoc />
    public partial class manytomany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CustomerShops",
                columns: table => new
                {
                    CustomersId = table.Column<int>(type: "int", nullable: false),
                    ShopsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerShops", x => new { x.CustomersId, x.ShopsId });
                    table.ForeignKey(
                        name: "FK_CustomerShops_Customers_CustomersId",
                        column: x => x.CustomersId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerShops_Shops_ShopsId",
                        column: x => x.ShopsId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MallCustomers",
                columns: table => new
                {
                    CustomersId = table.Column<int>(type: "int", nullable: false),
                    MallsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MallCustomers", x => new { x.CustomersId, x.MallsId });
                    table.ForeignKey(
                        name: "FK_MallCustomers_Customers_CustomersId",
                        column: x => x.CustomersId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MallCustomers_Malls_MallsId",
                        column: x => x.MallsId,
                        principalTable: "Malls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerShops_ShopsId",
                table: "CustomerShops",
                column: "ShopsId");

            migrationBuilder.CreateIndex(
                name: "IX_MallCustomers_MallsId",
                table: "MallCustomers",
                column: "MallsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerShops");

            migrationBuilder.DropTable(
                name: "MallCustomers");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
