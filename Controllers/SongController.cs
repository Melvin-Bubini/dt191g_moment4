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
        public async Task<ActionResult<IEnumerable<SongDto>>> GetSongs()
        {
            var songs = await _context.Songs
        .Include(s => s.Album)  // Inkludera albumet när du hämtar låten
        .ToListAsync();

            if (songs == null)
            {
                return NotFound();
            }

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
        public async Task<ActionResult<SongDto>> GetSong(int id)
        {
            var song = await _context.Songs
                .Include(s => s.Album)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (song == null)
            {
                return NotFound();
            }

            var songDto = new SongDto
            {
                Id = song.Id,
                Artist = song.Artist,
                Title = song.Title,
                Length = song.Length,
                Category = song.Category,
                AlbumId = song.AlbumId,
                Album = song.Album != null ? new AlbumDto
                {
                    Id = song.Album.Id,
                    Name = song.Album.Name
                } : null
            };

            return Ok(songDto);
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

            // Hämta befintlig låt för att hantera Album-relationen separat
            var existingSong = await _context.Songs
                .Include(s => s.Album)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (existingSong == null)
            {
                return NotFound();
            }

            // Uppdatera basegenskaper
            existingSong.Artist = song.Artist;
            existingSong.Title = song.Title;
            existingSong.Length = song.Length;
            existingSong.Category = song.Category;

            // Hantera album på liknande sätt som i PostSong
            if (song.Album != null)
            {
                var existingAlbum = await _context.Albums
                    .FirstOrDefaultAsync(a => a.Name == song.Album.Name);

                if (existingAlbum != null)
                {
                    existingSong.Album = null;
                    existingSong.AlbumId = existingAlbum.Id;
                }
                else
                {
                    if (existingSong.Album != null)
                    {
                        _context.Entry(existingSong.Album).State = EntityState.Detached;
                    }
                    _context.Albums.Add(song.Album);
                    await _context.SaveChangesAsync();

                    existingSong.AlbumId = song.Album.Id;
                }
            }
            else
            {
                existingSong.Album = null;
                existingSong.AlbumId = null;
            }

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
            // Om sång eller sång.Album är null, returnera BadRequest
            if (song == null)
            {
                return BadRequest("Song är null.");
            }

            // Om albumet är specificerat
            if (song.Album != null)
            {
                // Kolla om albumet redan finns i databasen
                var existingAlbum = await _context.Albums
                    .FirstOrDefaultAsync(a => a.Name == song.Album.Name);

                if (existingAlbum != null)
                {
                    // Om albumet finns, koppla låten till det albumet
                    song.Album = null; // Undvik att skapa dubletter
                    song.AlbumId = existingAlbum.Id;
                }
                else
                {
                    // Om albumet inte finns, skapa ett nytt album och koppla låten till det
                    _context.Albums.Add(song.Album);
                    await _context.SaveChangesAsync();  // Spara albumet för att få ett Id

                    song.AlbumId = song.Album.Id;  // Koppla låten till det nya albumet
                }
            }
            // Om inget album är specificerat, låt AlbumId vara null

            _context.Songs.Add(song);
            await _context.SaveChangesAsync();

            var createdSongDto = new SongDto
            {
                Id = song.Id,
                Artist = song.Artist,
                Title = song.Title,
                Length = song.Length,
                Category = song.Category,
                AlbumId = song.AlbumId,
                Album = song.Album != null ? new AlbumDto
                {
                    Id = song.Album.Id,
                    Name = song.Album.Name
                } : null
            };

            return CreatedAtAction("GetSong", new { id = song.Id }, createdSongDto);
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
