
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieApp.Models;

namespace MovieApp.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Movie> Movies { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

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
                      Name = "The Great Adventure",
                      ReleaseYear = 2001,
                      Director = "John Smith",
                      Description = "A thrilling journey through uncharted lands.",
                      Duration = 130,
                      Rating = 8.2,
                      ListPrice = 29.99,
                      Price = 24.99,
                      Price5 = 22.99,
                      Price10 = 19.99,
                      YoutubeId = "",
                      CategoryId = 1, // Action
                      ImageUrl = ""
                  },
                    new Movie
                    {
                        Id = 2,
                        Name = "Love in Paris",
                        ReleaseYear = 2011,
                        Director = "Emily Johnson",
                        Description = "A heartfelt romantic story set in Paris.",
                        Duration = 115,
                        Rating = 7.5,
                        ListPrice = 24.99,
                        Price = 19.99,
                        Price5 = 17.99,
                        Price10 = 15.99,
                        YoutubeId = "",
                        CategoryId = 2, // Romance
                        ImageUrl = ""
                    },
                    new Movie
                    {
                        Id = 3,
                        Name = "The Laugh Factory",
                        ReleaseYear = 2004,
                        Director = "Mike Chang",
                        Description = "Comedy that will leave you in stitches.",
                        Duration = 98,
                        Rating = 7.8,
                        ListPrice = 19.99,
                        Price = 15.99,
                        Price5 = 13.99,
                        Price10 = 11.99,
                        YoutubeId = "",
                        CategoryId = 3, // Comedy
                        ImageUrl = ""
                    },
                    new Movie
                    {
                        Id = 4,
                        Name = "Beyond the Stars",
                        ReleaseYear = 2015,
                        Director = "Samantha Lee",
                        Description = "A science fiction epic exploring distant galaxies.",
                        Duration = 145,
                        Rating = 8.6,
                        ListPrice = 34.99,
                        Price = 28.99,
                        Price5 = 26.99,
                        Price10 = 23.99,
                        YoutubeId = "",
                        CategoryId = 4, // Science Fiction
                        ImageUrl = ""
                    },
                    new Movie
                    {
                        Id = 5,
                        Name = "Haunted Echoes",
                        ReleaseYear = 2022,
                        Director = "Richard Black",
                        Description = "A chilling horror story that will keep you up at night.",
                        Duration = 105,
                        Rating = 6.9,
                        ListPrice = 22.99,
                        Price = 18.99,
                        Price5 = 16.99,
                        Price10 = 14.99,
                        YoutubeId = "",
                        CategoryId = 1, // Horror
                        ImageUrl = ""
                    }
                 );
        }
    }
}