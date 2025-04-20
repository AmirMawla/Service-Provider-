using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ServiceProvider_DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationUserData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ApplicationUsers",
                columns: new[] { "Id", "Address", "BirthDate", "Email", "FullName", "PhoneNumber", "RegistrationDate" },
                values: new object[,]
                {
                    { "abcdedvdvfdsfgh", "sharkia", null, "amir@email.com", "amir elsayed", "01002694473", null },
                    { "abcdefg", "sharkia", null, "ahmed@email.com", "ahmed tahoon", "01002694473", null },
                    { "fsjnvjkdsfjsfnvf", "sharkia", null, "hossam@email.com", "Hossam mostafa", "01002694473", null },
                    { "jndsjknfjmfifimkf", "sharkia", null, "fathi@email.com", "ahmed fathi", "01002694473", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ApplicationUsers",
                keyColumn: "Id",
                keyValue: "abcdedvdvfdsfgh");

            migrationBuilder.DeleteData(
                table: "ApplicationUsers",
                keyColumn: "Id",
                keyValue: "abcdefg");

            migrationBuilder.DeleteData(
                table: "ApplicationUsers",
                keyColumn: "Id",
                keyValue: "fsjnvjkdsfjsfnvf");

            migrationBuilder.DeleteData(
                table: "ApplicationUsers",
                keyColumn: "Id",
                keyValue: "jndsjknfjmfifimkf");
        }
    }
}
