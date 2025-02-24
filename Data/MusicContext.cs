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

    }
    }
}