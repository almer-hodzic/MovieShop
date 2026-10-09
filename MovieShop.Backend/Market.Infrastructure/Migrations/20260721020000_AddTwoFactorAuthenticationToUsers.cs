using System;
using Market.Infrastructure.Database;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Market.Infrastructure.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20260721020000_AddTwoFactorAuthenticationToUsers")]
    public partial class AddTwoFactorAuthenticationToUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTwoFactorEnabled",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "TwoFactorCodeExpiresAtUtc",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TwoFactorCodeHash",
                table: "Users",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TwoFactorFailedAttempts",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "IsTwoFactorEnabled", table: "Users");
            migrationBuilder.DropColumn(name: "TwoFactorCodeExpiresAtUtc", table: "Users");
            migrationBuilder.DropColumn(name: "TwoFactorCodeHash", table: "Users");
            migrationBuilder.DropColumn(name: "TwoFactorFailedAttempts", table: "Users");
        }
    }
}
