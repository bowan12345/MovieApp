
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
                      Name = "Inception",
                      Director = "Christopher Nolan",
                      Description = "This mind-bending sci-fi thriller follows a skilled thief, Dom Cobb, who is able to steal secrets from within a person's subconscious while they dream",
                      Duration = 130,
                      Rating = 8.2,
                      ListPrice = 29.99,
                      Price = 24.99,
                      Price5 = 22.99,
                      Price10 = 19.99,
                      YoutubeId = "YoHD9XEInc0",
                      CategoryId = 4, // Action
                      ImageUrl = ""
                  },
                    new Movie
                    {
                        Id = 2,
                        Name = "Love in Paris",
                        Director = "Emily Johnson",
                        Description = "A heartfelt romantic story set in Paris.",
                        Duration = 115,
                        Rating = 7.5,
                        ListPrice = 24.99,
                        Price = 19.99,
                        Price5 = 17.99,
                        Price10 = 15.99,
                        YoutubeId = "lhMcYv0PG6o",
                        CategoryId = 3, 
                        ImageUrl = ""
                    },
                    new Movie
                    {
                        Id = 3,
                        Name = "The Dark Knight",
                        Director = "Christopher Nolan",
                        Description = "The second installment of Nolan’s Batman trilogy, this film follows Batman as he faces off against the Joker,",
                        Duration = 98,
                        Rating = 7.8,
                        ListPrice = 19.99,
                        Price = 15.99,
                        Price5 = 13.99,
                        Price10 = 11.99,
                        YoutubeId = "EXeTwQWrcwY",
                        CategoryId = 1, 
                        ImageUrl = ""
                    },
                    new Movie
                    {
                        Id = 4,
                        Name = "Beyond the Stars",
                        Director = "Samantha Lee",
                        Description = "A science fiction epic exploring distant galaxies.",
                        Duration = 145,
                        Rating = 8.6,
                        ListPrice = 34.99,
                        Price = 28.99,
                        Price5 = 26.99,
                        Price10 = 23.99,
                        YoutubeId = "mBrVtZZeIm8",
                        CategoryId = 4,
                        ImageUrl = ""
                    },
                    new Movie
                    {
                        Id = 5,
                        Name = "Fight Club",
                        Director = "David Fincher",
                        Description = "Based on Chuck Palahniuk's novel, Fight Club follows an unnamed protagonist who, disillusioned with his mundane life, meets the charismatic Tyler Durden",
                        Duration = 105,
                        Rating = 6.9,
                        ListPrice = 22.99,
                        Price = 18.99,
                        Price5 = 16.99,
                        Price10 = 14.99,
                        YoutubeId = "qtRKdVHc-cE",
                        CategoryId = 1, 
                        ImageUrl = ""
                    }
                 );
        }
    }
}
