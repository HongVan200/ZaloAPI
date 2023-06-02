using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZaloMiniAppAPI.Migrations
{
    /// <inheritdoc />
    public partial class MYPAGE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Orders_OrdersID",
                table: "CartItems");

            migrationBuilder.DropIndex(
                name: "IX_CartItems_OrdersID",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "OrdersID",
                table: "CartItems");

            migrationBuilder.CreateTable(
                name: "ProDuctOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProDuctOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetail",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    ProDuctOrdersId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetail", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_OrderDetail_Orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetail_ProDuctOrders_ProDuctOrdersId",
                        column: x => x.ProDuctOrdersId,
                        principalTable: "ProDuctOrders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderDetail_Products_ProductID",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_OrderID",
                table: "OrderDetail",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_ProductID",
                table: "OrderDetail",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_ProDuctOrdersId",
                table: "OrderDetail",
                column: "ProDuctOrdersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderDetail");

            migrationBuilder.DropTable(
                name: "ProDuctOrders");

            migrationBuilder.AddColumn<int>(
                name: "OrdersID",
                table: "CartItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_OrdersID",
                table: "CartItems",
                column: "OrdersID");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Orders_OrdersID",
                table: "CartItems",
                column: "OrdersID",
                principalTable: "Orders",
                principalColumn: "ID");
        }
    }
}
