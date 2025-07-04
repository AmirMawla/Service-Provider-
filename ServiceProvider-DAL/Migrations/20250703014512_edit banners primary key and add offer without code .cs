using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceProvider_DAL.Migrations
{
    /// <inheritdoc />
    public partial class editbannersprimarykeyandaddofferwithoutcode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Banners",
                table: "Banners");

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountPercentage",
                table: "Banners",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DiscountCode",
                table: "Banners",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Banners",
                table: "Banners",
                columns: new[] { "ProductId", "VendorId", "DiscountPercentage" });
        }

           

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.DropPrimaryKey(
                name: "PK_Banners",
                table: "Banners");

            migrationBuilder.AlterColumn<string>(
                name: "DiscountCode",
                table: "Banners",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountPercentage",
                table: "Banners",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Banners",
                table: "Banners",
                columns: new[] { "ProductId", "VendorId", "DiscountCode" });
        }
    }
}
