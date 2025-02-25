using Microsoft.EntityFrameworkCore;
using dt191g_moment4.Models;
namespace dt191g_moment4.Data
{

    public class MusicContext : DbContext
    {
        public MusicContext(DbContextOptions<MusicContext> options) : base(options)
        {

        }

        public DbSet<Song> Songs { get; set; }
        public DbSet<Album> Albums { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Definiera relationen mellan Song och Album
            modelBuilder.Entity<Song>()
                .HasOne(s => s.Album)      // Låt har ett Album
                .WithMany(a => a.Songs)    // Album har många låtar
                .HasForeignKey(s => s.AlbumId)  // AlbumId är främmande nyckel
                .OnDelete(DeleteBehavior.SetNull); // Vid radering sätt AlbumId till null istället för att radera låten

            // Testdata
            modelBuilder.Entity<Album>().HasData(
                new Album { Id = 1, Name = "Divide" },
                new Album { Id = 2, Name = "Thriller" },
                new Album { Id = 3, Name = "Back in Black" },
                new Album { Id = 4, Name = "Abbey Road" },
                new Album { Id = 5, Name = "Future Nostalgia" }
            );

            modelBuilder.Entity<Song>().HasData(
                new Song { Id = 1, Artist = "Ed Sheeran", Title = "Shape of You", Length = 233, Category = "Pop", AlbumId = 1 },
                new Song { Id = 2, Artist = "Ed Sheeran", Title = "Perfect", Length = 263, Category = "Pop", AlbumId = 1 },

                new Song { Id = 3, Artist = "Michael Jackson", Title = "Billie Jean", Length = 294, Category = "Pop", AlbumId = 2 },
                new Song { Id = 4, Artist = "Michael Jackson", Title = "Thriller", Length = 358, Category = "Pop", AlbumId = 2 },

                new Song { Id = 5, Artist = "AC/DC", Title = "Back in Black", Length = 255, Category = "Rock", AlbumId = 3 },
                new Song { Id = 6, Artist = "AC/DC", Title = "You Shook Me All Night Long", Length = 210, Category = "Rock", AlbumId = 3 },

                new Song { Id = 7, Artist = "The Beatles", Title = "Come Together", Length = 259, Category = "Rock", AlbumId = 4 },
                new Song { Id = 8, Artist = "The Beatles", Title = "Here Comes the Sun", Length = 185, Category = "Rock", AlbumId = 4 },

                new Song { Id = 9, Artist = "Dua Lipa", Title = "Don't Start Now", Length = 183, Category = "Pop", AlbumId = 5 },
                new Song { Id = 10, Artist = "Dua Lipa", Title = "Levitating", Length = 203, Category = "Pop", AlbumId = 5 },

                // En låt utan album
                new Song { Id = 11, Artist = "Adele", Title = "Hello", Length = 295, Category = "Pop", AlbumId = null }
            );
        }
    }
}