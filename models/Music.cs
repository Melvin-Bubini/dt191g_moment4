using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace dt191g_moment4.Models
{

    public class Song
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Artist är obligatoriskt")]
        [StringLength(50, ErrorMessage = "Artist får inte överstiga 50 tecken")]
        public string Artist { get; set; } = string.Empty;

        [Required(ErrorMessage = "Titel är obligatoriskt")]
        [StringLength(50, ErrorMessage = "Titel får inte överstiga 50 tecken")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Längd är obligatoriskt")]
        [Range(1, 3600, ErrorMessage = "Längd måste vara mellan 1 och 3600 sekunder")]
        public int Length { get; set; }

        [Required(ErrorMessage = "Kategori är obligatoriskt")]
        [StringLength(50, ErrorMessage = "Kategori får inte överstiga 50 tecken")]
        public string Category { get; set; } = string.Empty;

        public int? AlbumId { get; set; }
        [JsonPropertyName("album")]
        public Album? Album { get; set; }
    }

    public class Album
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Album namn är obligatoriskt")]
        [StringLength(50, ErrorMessage = "Album namn kan inte vara längre än 50 tecken.")]
        public string Name { get; set; } = null!;
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
