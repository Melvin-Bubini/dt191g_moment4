using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace dt191g_moment4.Models
{

    public class Song
    {
        public int Id { get; set; }

        [Required]
        public string Artist { get; set; } = string.Empty;
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public int Length { get; set; }
        [Required]
        public string Category { get; set; } = string.Empty;

        public int? AlbumId { get; set; }
        [JsonPropertyName("album")]
        public Album? Album { get; set; }
    }

    public class Album
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [JsonIgnore]
        public List<Song> Songs { get; set; } = new List<Song>();
    }

    public class SongDto
    {
        public int Id { get; set; }
        public string Artist { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Length { get; set; }
        public string Category { get; set; } = string.Empty;
        public int? AlbumId { get; set; }
        public AlbumDto? Album { get; set; }
    }

    public class AlbumDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

}
