using Market.Infrastructure.Database;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Market.Infrastructure.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20261010000000_AddUsernameToUsers")]
    public partial class AddUsernameToUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE [Users]
                SET [Username] = 'admin'
                WHERE LOWER([Email]) = 'admin@market.local'
                  AND ([Username] IS NULL OR LTRIM(RTRIM([Username])) = '')
                """);

            migrationBuilder.Sql("""
                UPDATE [Users]
                SET [Username] = 'user'
                WHERE LOWER([Email]) = 'user@market.local'
                  AND ([Username] IS NULL OR LTRIM(RTRIM([Username])) = '')
                """);

            migrationBuilder.Sql("""
                UPDATE [Users]
                SET [Username] = CONCAT('user', [Id])
                WHERE [Username] IS NULL OR LTRIM(RTRIM([Username])) = ''
                """);

            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Username",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "Users");
        }
    }
}
