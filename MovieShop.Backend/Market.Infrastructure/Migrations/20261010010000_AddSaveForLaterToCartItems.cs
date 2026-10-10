using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Market.Infrastructure.Migrations
{
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
