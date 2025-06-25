using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceProvider_DAL.Migrations
{
    /// <inheritdoc />
    public partial class afterupdateshipement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Shippings",
                table: "Shippings");

            migrationBuilder.DropIndex(
                name: "IX_Shippings_OrderId",
                table: "Shippings");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Shippings");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Shippings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<string>(
                name: "VendorId",
                table: "Shippings",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shippings",
                table: "Shippings",
                columns: new[] { "OrderId", "VendorId" });

            migrationBuilder.CreateIndex(
                name: "IX_Shippings_VendorId",
                table: "Shippings",
                column: "VendorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shippings_AspNetUsers_VendorId",
                table: "Shippings",
                column: "VendorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shippings_AspNetUsers_VendorId",
                table: "Shippings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shippings",
                table: "Shippings");

            migrationBuilder.DropIndex(
                name: "IX_Shippings_VendorId",
                table: "Shippings");

            migrationBuilder.DropColumn(
                name: "VendorId",
                table: "Shippings");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Shippings",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Shippings",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shippings",
                table: "Shippings",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Shippings_OrderId",
                table: "Shippings",
                column: "OrderId",
                unique: true);
        }
    }
}
