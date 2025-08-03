using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MipWeb.Migrations
{
    /// <inheritdoc />
    public partial class WhatsAppOtp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPhoneVerified",
                table: "Students",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "OtpVerifications",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPhoneVerified",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "OtpVerifications");
        }
    }
}
