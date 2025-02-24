using Microsoft.EntityFrameworkCore;
using dt191g_moment4.Models;
namespace dt191g_moment4.Data {

    public class SongContext : DbContext {
        public SongContext(DbContextOptions<SongContext> options) : base(options) {

        }

        public DbSet<Song> Songs { get; set; }
    }
}