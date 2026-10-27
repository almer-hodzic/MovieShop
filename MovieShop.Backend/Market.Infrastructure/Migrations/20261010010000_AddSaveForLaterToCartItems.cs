using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Market.Infrastructure.Database;

#nullable disable

namespace Market.Infrastructure.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20261010010000_AddSaveForLaterToCartItems")]
    public partial class AddSaveForLaterToCartItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CartItems_ShoppingCartId_MovieId",
                table: "CartItems");

            migrationBuilder.AddColumn<bool>(
                name: "IsSavedForLater",
                table: "CartItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ShoppingCartId_MovieId_IsSavedForLater",
                table: "CartItems",
                columns: new[] { "ShoppingCartId", "MovieId", "IsSavedForLater" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CartItems_ShoppingCartId_MovieId_IsSavedForLater",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "IsSavedForLater",
                table: "CartItems");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ShoppingCartId_MovieId",
                table: "CartItems",
                columns: new[] { "ShoppingCartId", "MovieId" });
        }
    }
}
