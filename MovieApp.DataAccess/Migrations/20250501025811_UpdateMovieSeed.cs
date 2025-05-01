using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MovieApp.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMovieSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "Id", "CategoryId", "Description", "Director", "Duration", "ImageUrl", "ListPrice", "Name", "Price", "Price10", "Price5", "Rating", "ReleaseYear", "YoutubeId" },
                values: new object[,]
                {
                    { 6, 1, "Banker Andy Dufresne is wrongfully imprisoned and befriends fellow inmate Red at Shawshank prison. ", "Frank Darabont", 142, "", 22.989999999999998, "The Shawshank Redemption", 18.989999999999998, 14.99, 16.989999999999998, 6.9000000000000004, 1994, "PLl99DlL6b4" },
                    { 7, 1, "Neo, a computer hacker, discovers that the world he lives in is a simulated reality controlled by machines.", "The Wachowskisr", 136, "", 22.989999999999998, "The Matrix", 18.989999999999998, 14.99, 16.989999999999998, 6.9000000000000004, 1999, "vKQi3bBA1y8" },
                    { 8, 1, "The powerful and influential Corleone mafia family is headed by Don Vito Corleone, whose youngest son Michael gradually gets drawn into the family business.", "Francis Ford Coppola", 175, "", 22.989999999999998, "The Godfather", 18.989999999999998, 14.99, 16.989999999999998, 6.9000000000000004, 1972, "UaVTIH8mujA" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 8);
        }
    }
}
