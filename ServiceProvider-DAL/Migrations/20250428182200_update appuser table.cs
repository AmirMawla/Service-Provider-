using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceProvider_DAL.Migrations
{
    /// <inheritdoc />
    public partial class updateappusertable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "ApplicationUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ApplicationUsers",
                keyColumn: "Id",
                keyValue: "abcdedvdvfdsfgh",
                column: "ImageUrl",
                value: "");

            migrationBuilder.UpdateData(
                table: "ApplicationUsers",
                keyColumn: "Id",
                keyValue: "abcdefg",
                column: "ImageUrl",
                value: "");

            migrationBuilder.UpdateData(
                table: "ApplicationUsers",
                keyColumn: "Id",
                keyValue: "fsjnvjkdsfjsfnvf",
                column: "ImageUrl",
                value: "");

            migrationBuilder.UpdateData(
                table: "ApplicationUsers",
                keyColumn: "Id",
                keyValue: "jndsjknfjmfifimkf",
                column: "ImageUrl",
                value: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "ApplicationUsers");
        }
    }
}
