using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MovieApp.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Director { get; set; }
        public string? Description { get; set; }
        [Required]
        public int Duration { get; set; }//Duration in minutes
        [Required]
        [Range(0, 10, ErrorMessage = "Rating should be between 0 and 10")]
        public double Rating { get; set; }//Rating score

        [Required]
        [Range(1900, 2100, ErrorMessage = "ReleaseYear should be between 1900 and 2100")]
        public int ReleaseYear { get; set; }//Release Year
        [Required]
        [Display(Name = "List Price")] //List Price
        [Range(1, 1000)]
        public double ListPrice { get; set; }
        [Required]
        [Range(1, 1000)]
        public double Price { get; set; }

        #region extended properties for extension
        [Required]
        [Display(Name = "Price for 6-10")]
        [Range(1, 1000)]
        public double Price5 { get; set; }
        [Required]
        [Display(Name = "Price for 10+")]
        [Range(1, 1000)]
        public double Price10 { get; set; }
        #endregion

        //youtube video id
        public string? YoutubeId { get; set; }
        //foreign key
        public int CategoryId { get; set; }
        //navigation property
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }
        [ValidateNever]
        public string? ImageUrl { get; set; }

        // Navigation property for MovieVotes
        [ValidateNever]
        public virtual ICollection<MovieVote> MovieVotes { get; set; } = new List<MovieVote>();

        [NotMapped]
        public int LikeCount { get; set; }
        [NotMapped]
        public int DislikeCount { get; set; }
    }
}