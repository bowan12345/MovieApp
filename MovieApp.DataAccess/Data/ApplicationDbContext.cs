
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MovieApp.Models;

namespace BulkyWeb.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Movie> Movies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayOrder = 1, Description = "Movies with high energy, physical stunts, and intense scenes." },
                new Category { Id = 2, Name = "Comedy", DisplayOrder = 2, Description = "Movies designed to make the audience laugh." },
                new Category { Id = 3, Name = "Drama", DisplayOrder = 3, Description = "Serious, emotional storytelling with character development." },
                new Category { Id = 4, Name = "Science Fiction", DisplayOrder = 4, Description = "Futuristic or science-based themes, often with technology or space." }
                );


            modelBuilder.Entity<Movie>().HasData(
                  new Movie
                  {
                      Id = 1,
                      Title = "The Great Adventure",
                      Director = "John Smith",
                      Description = "A thrilling journey through uncharted lands.",
                      Duration = 130,
                      Rating = 8.2,
                      ListPrice = 29.99,
                      Price = 24.99,
                      Price50 = 22.99,
                      Price100 = 19.99,
                      CategoryId = 1, // Action
                      ImageUrl = ""
                  },
                    new Movie
                    {
                        Id = 2,
                        Title = "Love in Paris",
                        Director = "Emily Johnson",
                        Description = "A heartfelt romantic story set in Paris.",
                        Duration = 115,
                        Rating = 7.5,
                        ListPrice = 24.99,
                        Price = 19.99,
                        Price50 = 17.99,
                        Price100 = 15.99,
                        CategoryId = 2, // Romance
                        ImageUrl = ""
                    },
                    new Movie
                    {
                        Id = 3,
                        Title = "The Laugh Factory",
                        Director = "Mike Chang",
                        Description = "Comedy that will leave you in stitches.",
                        Duration = 98,
                        Rating = 7.8,
                        ListPrice = 19.99,
                        Price = 15.99,
                        Price50 = 13.99,
                        Price100 = 11.99,
                        CategoryId = 3, // Comedy
                        ImageUrl = ""
                    },
                    new Movie
                    {
                        Id = 4,
                        Title = "Beyond the Stars",
                        Director = "Samantha Lee",
                        Description = "A science fiction epic exploring distant galaxies.",
                        Duration = 145,
                        Rating = 8.6,
                        ListPrice = 34.99,
                        Price = 28.99,
                        Price50 = 26.99,
                        Price100 = 23.99,
                        CategoryId = 4, // Science Fiction
                        ImageUrl = ""
                    },
                    new Movie
                    {
                        Id = 5,
                        Title = "Haunted Echoes",
                        Director = "Richard Black",
                        Description = "A chilling horror story that will keep you up at night.",
                        Duration = 105,
                        Rating = 6.9,
                        ListPrice = 22.99,
                        Price = 18.99,
                        Price50 = 16.99,
                        Price100 = 14.99,
                        CategoryId = 1, // Horror
                        ImageUrl = ""
                    }
                 );
        }
    }
}
