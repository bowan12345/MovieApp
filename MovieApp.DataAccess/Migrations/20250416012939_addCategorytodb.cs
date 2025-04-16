using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MovieApp.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addCategorytodb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Director = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<double>(type: "float", nullable: false),
                    ListPrice = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Price50 = table.Column<double>(type: "float", nullable: false),
                    Price100 = table.Column<double>(type: "float", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movies_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "DisplayOrder", "Name" },
                values: new object[,]
                {
                    { 1, "Movies with high energy, physical stunts, and intense scenes.", 1, "Action" },
                    { 2, "Movies designed to make the audience laugh.", 2, "Comedy" },
                    { 3, "Serious, emotional storytelling with character development.", 3, "Drama" },
                    { 4, "Futuristic or science-based themes, often with technology or space.", 4, "Science Fiction" }
                });

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "Id", "CategoryId", "Description", "Director", "Duration", "ImageUrl", "ListPrice", "Price", "Price100", "Price50", "Rating", "Title" },
                values: new object[,]
                {
                    { 1, 1, "A thrilling journey through uncharted lands.", "John Smith", 130, "", 29.989999999999998, 24.989999999999998, 19.989999999999998, 22.989999999999998, 8.1999999999999993, "The Great Adventure" },
                    { 2, 2, "A heartfelt romantic story set in Paris.", "Emily Johnson", 115, "", 24.989999999999998, 19.989999999999998, 15.99, 17.989999999999998, 7.5, "Love in Paris" },
                    { 3, 3, "Comedy that will leave you in stitches.", "Mike Chang", 98, "", 19.989999999999998, 15.99, 11.99, 13.99, 7.7999999999999998, "The Laugh Factory" },
                    { 4, 4, "A science fiction epic exploring distant galaxies.", "Samantha Lee", 145, "", 34.990000000000002, 28.989999999999998, 23.989999999999998, 26.989999999999998, 8.5999999999999996, "Beyond the Stars" },
                    { 5, 1, "A chilling horror story that will keep you up at night.", "Richard Black", 105, "", 22.989999999999998, 18.989999999999998, 14.99, 16.989999999999998, 6.9000000000000004, "Haunted Echoes" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movies_CategoryId",
                table: "Movies",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
