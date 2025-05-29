using Microsoft.AspNetCore.Mvc.Rendering;
using MovieApp.Models;
using System.Collections.Generic;

namespace MovieApp.Models.ViewModels
{
    public class ReviewVM
    {
        public Review Review { get; set; }
        public IEnumerable<SelectListItem> MovieList { get; set; }
    }
}