using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Models
{
    [Table("MovieVotes")]
    public class MovieVote
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int MovieId { get; set; }

        [Required]
        [MaxLength(450)] // Standard AspNetUser Id length
        public string UserId { get; set; }

        [Required]
        public bool IsLike { get; set; } // true for like, false for dislike

        [Required]
        public DateTime VotedAt { get; set; } = DateTime.Now;

        // Navigation properties (optional - not required for basic functionality)
        // Uncomment these if you need navigation properties
        // [ForeignKey("MovieId")]
        // public virtual Movie Movie { get; set; }

        // [ForeignKey("UserId")]  
        // public virtual ApplicationUser User { get; set; }
    }
}