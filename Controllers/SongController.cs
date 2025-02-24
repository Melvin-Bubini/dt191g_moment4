using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using dt191g_moment4.Data;
using dt191g_moment4.Models;

namespace dt191g_moment4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongController : ControllerBase
    {
        private readonly MusicContext _context;

        public SongController(MusicContext context)
        {
            _context = context;
        }

        // GET: api/Song
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Song>>> GetSongs()
        {
            var songs = await _context.Songs
        .Include(s => s.Album)  // Inkludera albumet när du hämtar låten
        .ToListAsync();

            var songDtos = songs.Select(song => new SongDto
            {
                Id = song.Id,
                Artist = song.Artist,
                Title = song.Title,
                Length = song.Length,
                Category = song.Category,
                AlbumId = song.AlbumId ?? 0,
                Album = song.Album != null ? new AlbumDto
                {
                    Id = song.Album.Id,
                    Name = song.Album.Name ?? string.Empty
                } : null
            }).ToList();

            return Ok(songDtos);
        }

        // GET: api/Song/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Song>> GetSong(int id)
        {
            var song = await _context.Songs.FindAsync(id);

            if (song == null)
            {
                return NotFound();
            }

            return song;
        }

        // PUT: api/Song/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSong(int id, Song song)
        {
            if (id != song.Id)
            {
                return BadRequest();
            }

            _context.Entry(song).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SongExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Song
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Song>> PostSong([FromBody] Song song)
        {
            Console.WriteLine($"Received Song: {song}");
            Console.WriteLine($"Album: {song?.Album}");
            Console.WriteLine($"Album Name: {song?.Album?.Name}");
            // Om sång eller sång.Album är null, returnera BadRequest
            if (song == null || song.Album == null || string.IsNullOrWhiteSpace(song.Album.Name))
            {
                return BadRequest("Ogiltig sång data: Saknar album.");
            }

            // Kolla om albumet redan finns i databasen
            var existingAlbum = await _context.Albums
                .FirstOrDefaultAsync(a => a.Name == song.Album.Name);

            if (existingAlbum != null)
            {
                // Om albumet finns, koppla låten till det albumet
                song.AlbumId = existingAlbum.Id;
            }
            else if (song.Album != null)
            {
                // Om albumet inte finns, skapa ett nytt album och koppla låten till det
                _context.Albums.Add(song.Album);
                await _context.SaveChangesAsync();  // Spara albumet för att få ett Id

                song.AlbumId = song.Album.Id;  // Koppla låten till det nya albumet
            }

            _context.Songs.Add(song);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSong", new { id = song.Id }, song);
        }

        // DELETE: api/Song/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSong(int id)
        {
            var song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }

            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SongExists(int id)
        {
            return _context.Songs.Any(e => e.Id == id);
        }
    }
}
