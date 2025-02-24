using System.ComponentModel.DataAnnotations;

namespace dt191g_moment4.Models
{

    public class Song
    {
        public int Id { get; set; }

        [Required]
        public string? Artist { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public int Length { get; set; }
        [Required]
        public string? category { get; set; }
    }

    public class Album {
        public int Id { get; set; }

        [Required]
        public Song? Song { get; set; }
    }
}
