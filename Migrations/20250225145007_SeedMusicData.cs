using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace dt191g_moment4.Migrations
{
    /// <inheritdoc />
    public partial class SeedMusicData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Albums",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Divide" },
                    { 2, "Thriller" },
                    { 3, "Back in Black" },
                    { 4, "Abbey Road" },
                    { 5, "Future Nostalgia" }
                });

            migrationBuilder.InsertData(
                table: "Songs",
                columns: new[] { "Id", "AlbumId", "Artist", "Category", "Length", "Title" },
                values: new object[,]
                {
                    { 11, null, "Adele", "Pop", 295, "Hello" },
                    { 1, 1, "Ed Sheeran", "Pop", 233, "Shape of You" },
                    { 2, 1, "Ed Sheeran", "Pop", 263, "Perfect" },
                    { 3, 2, "Michael Jackson", "Pop", 294, "Billie Jean" },
                    { 4, 2, "Michael Jackson", "Pop", 358, "Thriller" },
                    { 5, 3, "AC/DC", "Rock", 255, "Back in Black" },
                    { 6, 3, "AC/DC", "Rock", 210, "You Shook Me All Night Long" },
                    { 7, 4, "The Beatles", "Rock", 259, "Come Together" },
                    { 8, 4, "The Beatles", "Rock", 185, "Here Comes the Sun" },
                    { 9, 5, "Dua Lipa", "Pop", 183, "Don't Start Now" },
                    { 10, 5, "Dua Lipa", "Pop", 203, "Levitating" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Songs",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Albums",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Albums",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Albums",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Albums",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Albums",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
