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
        public double Rating { get; set; }//Rating score

        [Required]
        [Display(Name = "List Price")] //List Price
        [Range(1, 1000)]
        public double ListPrice { get; set; }

        [Required]
        [Display(Name = "Price for 1-5")]
        [Range(1, 1000)]
        public double Price { get; set; }

        [Required]
        [Display(Name = "Price for 5-10")]
        [Range(1, 1000)]
        public double Price5 { get; set; }

        [Required]
        [Display(Name = "Price for 11")]
        [Range(1, 1000)]
        public double Price10 { get; set; }


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
    }
}
