using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
        public string? Category { get; set; }

        public int? AlbumId { get; set; }
        public Album? Album { get; set; }
    }

    public class Album
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public List<Song> Songs { get; set; } = new List<Song>();
    }
}
