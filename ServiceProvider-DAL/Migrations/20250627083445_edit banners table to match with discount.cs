using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceProvider_DAL.Migrations
{
    /// <inheritdoc />
    public partial class editbannerstabletomatchwithdiscount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Banners",
                table: "Banners");

            migrationBuilder.DropIndex(
                name: "IX_Banners_ProductId",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Banners");

            migrationBuilder.AddColumn<string>(
                name: "DiscountCode",
                table: "Banners",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Banners",
                table: "Banners",
                columns: new[] { "ProductId", "VendorId", "DiscountCode" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Banners",
                table: "Banners");

            migrationBuilder.DropColumn(
                name: "DiscountCode",
                table: "Banners");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Banners",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Banners",
                table: "Banners",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Banners_ProductId",
                table: "Banners",
                column: "ProductId");
        }
    }
}
