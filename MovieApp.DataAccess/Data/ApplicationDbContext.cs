
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
                      ReleaseYear = 2010, 
                      Duration = 130,
                      Rating = 8.2,
                      ListPrice = 29.99,
                      Price = 24.99,
                      Price5 = 22.99,
                      Price10 = 19.99,
                      YoutubeId = "YoHD9XEInc0",
                      CategoryId = 4, // Action
                      ImageUrl = "\\images\\movie\\824295b2-2fa4-48e1-9055-5fb63b9b1ddf.jpg"
                  },
                    new Movie
                    {
                        Id = 2,
                        Name = "Love in Paris",
                        Director = "Emily Johnson",
                        Description = "A heartfelt romantic story set in Paris.",
                        ReleaseYear = 1997,
                        Duration = 115,
                        Rating = 7.5,
                        ListPrice = 24.99,
                        Price = 19.99,
                        Price5 = 17.99,
                        Price10 = 15.99,
                        YoutubeId = "lhMcYv0PG6o",
                        CategoryId = 3,
                        ImageUrl = "\\images\\movie\\83309ec0-a276-48d3-b673-bcbe55030fc3.jpg"
                    },
                    new Movie
                    {
                        Id = 3,
                        Name = "The Dark Knight",
                        Director = "Christopher Nolan",
                        Description = "The second installment of Nolan’s Batman trilogy, this film follows Batman as he faces off against the Joker,",
                        ReleaseYear = 2008,
                        Duration = 98,
                        Rating = 7.8,
                        ListPrice = 19.99,
                        Price = 15.99,
                        Price5 = 13.99,
                        Price10 = 11.99,
                        YoutubeId = "EXeTwQWrcwY",
                        CategoryId = 1,
                        ImageUrl = "\\images\\movie\\826c3850-f961-4b14-b8af-9c731ea8807f.jpg"
                    },
                    new Movie
                    {
                        Id = 4,
                        Name = "Beyond the Stars",
                        Director = "Samantha Lee",
                        Description = "A science fiction epic exploring distant galaxies.",
                        ReleaseYear = 1989,
                        Duration = 145,
                        Rating = 8.6,
                        ListPrice = 34.99,
                        Price = 28.99,
                        Price5 = 26.99,
                        Price10 = 23.99,
                        YoutubeId = "2t7z_44nGio",
                        CategoryId = 4,
                        ImageUrl = "\\images\\movie\\0ff60068-9f54-440a-be9a-c26c6ed64469.jpg"
                    },
                    new Movie
                    {
                        Id = 5,
                        Name = "Fight Club",
                        Director = "David Fincher",
                        Description = "Based on Chuck Palahniuk's novel, Fight Club follows an unnamed protagonist who, disillusioned with his mundane life, meets the charismatic Tyler Durden",
                        ReleaseYear = 1999,
                        Duration = 105,
                        Rating = 6.9,
                        ListPrice = 22.99,
                        Price = 18.99,
                        Price5 = 16.99,
                        Price10 = 14.99,
                        YoutubeId = "qtRKdVHc-cE",
                        CategoryId = 1,
                        ImageUrl = "\\images\\movie\\056287bf-89af-4ed0-bc8e-3b4a2f1ed642.jpg"
                    },
                    new Movie
                    {
                        Id = 6,
                        Name = "The Shawshank Redemption",
                        Director = "Frank Darabont",
                        Description = "Banker Andy Dufresne is wrongfully imprisoned and befriends fellow inmate Red at Shawshank prison. ",
                        ReleaseYear = 1994,
                        Duration = 142,
                        Rating = 6.9,
                        ListPrice = 22.99,
                        Price = 18.99,
                        Price5 = 16.99,
                        Price10 = 14.99,
                        YoutubeId = "PLl99DlL6b4",
                        CategoryId = 1,
                        ImageUrl = "\\images\\movie\\a42862fb-73a9-40bd-aa10-472af8477d64.jpg"
                    },
                    new Movie
                    {
                        Id = 7,
                        Name = "The Matrix",
                        Director = "The Wachowskisr",
                        Description = "Neo, a computer hacker, discovers that the world he lives in is a simulated reality controlled by machines.",
                        ReleaseYear = 1999,
                        Duration = 136,
                        Rating = 6.9,
                        ListPrice = 22.99,
                        Price = 18.99,
                        Price5 = 16.99,
                        Price10 = 14.99,
                        YoutubeId = "vKQi3bBA1y8",
                        CategoryId = 1,
                        ImageUrl = "\\images\\movie\\677d1b65-0a3d-4427-9b44-be008bc678e5.jpg"
                    },
                    new Movie
                    {
                        Id = 8,
                        Name = "The Godfather",
                        Director = "Francis Ford Coppola",
                        Description = "The powerful and influential Corleone mafia family is headed by Don Vito Corleone, whose youngest son Michael gradually gets drawn into the family business.",
                        ReleaseYear = 1972,
                        Duration = 175,
                        Rating = 6.9,
                        ListPrice = 22.99,
                        Price = 18.99,
                        Price5 = 16.99,
                        Price10 = 14.99,
                        YoutubeId = "UaVTIH8mujA",
                        CategoryId = 1,
                        ImageUrl = "\\images\\movie\\af91e03f-a44a-4545-9d04-d9ae1c138eba.jpg"
                    }
                 );
        }
    }
}