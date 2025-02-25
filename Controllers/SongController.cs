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
        private readonly ILogger<SongController> _logger;

        public SongController(MusicContext context, ILogger<SongController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Song
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SongDto>>> GetSongs()
        {
            try
            {
                var songs = await _context.Songs
                    .Include(s => s.Album)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ett fel inträffade vid hämtning av alla låtar");
                return StatusCode(500, "Ett internt serverfel inträffade vid hämtning av låtar.");
            }
        }

        // GET: api/Song/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SongDto>> GetSong(int id)
        {
            try
            {
                var song = await _context.Songs
                    .Include(s => s.Album)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (song == null)
                {
                    return NotFound($"Låten med id {id} hittades inte.");
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
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ett fel inträffade vid hämtning av låt med id {id}");
                return StatusCode(500, "Ett internt serverfel inträffade vid hämtning av låten.");
            }
        }

        // PUT: api/Song/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSong(int id, Song song)
        {
            if (song == null)
            {
                return BadRequest("Låten kan inte vara null.");
            }

            if (id != song.Id)
            {
                return BadRequest("ID i URL:en matchar inte ID i låtobjektet.");
            }

            if (string.IsNullOrEmpty(song.Title) || string.IsNullOrEmpty(song.Artist))
            {
                return BadRequest("Titel och artist måste finnas med.");
            }

            try
            {
                // Hämta befintlig låt för att hantera Album-relationen separat
                var existingSong = await _context.Songs
                    .Include(s => s.Album)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (existingSong == null)
                {
                    return NotFound($"Låten med id {id} hittades inte.");
                }

                // Kolla om den uppdaterade titeln och artisten redan finns för en annan låt
                var duplicateSong = await _context.Songs
                    .FirstOrDefaultAsync(s => s.Id != id && s.Title == song.Title && s.Artist == song.Artist);

                if (duplicateSong != null)
                {
                    return Conflict("En låt med denna artist och titel finns redan.");
                }

                // Uppdatera basegenskaper
                existingSong.Artist = song.Artist;
                existingSong.Title = song.Title;
                existingSong.Length = song.Length;
                existingSong.Category = song.Category;

                // Hantera album på liknande sätt som i PostSong
                if (song.Album != null)
                {
                    if (string.IsNullOrEmpty(song.Album.Name))
                    {
                        return BadRequest("Albumnamn kan inte vara tomt.");
                    }

                    var existingAlbum = await _context.Albums
                        .FirstOrDefaultAsync(a => a.Name == song.Album.Name);

                    if (existingAlbum != null)
                    {

                        existingSong.AlbumId = existingAlbum.Id;
                    }
                    else
                    {
                        // Skapa ett nytt album
                        var newAlbum = new Album { Name = song.Album.Name };
                        _context.Albums.Add(newAlbum);
                        await _context.SaveChangesAsync();

                        existingSong.AlbumId = newAlbum.Id;
                    }
                }
                else if (song.AlbumId.HasValue)
                {
                    // Om det finns ett AlbumId men inget Album-objekt,
                    // behåll bara AlbumId
                    existingSong.AlbumId = song.AlbumId;
                }
                else
                {
                    // Bara om det uttryckligen inte ska finnas något album
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
                        return NotFound($"Låten med id {id} hittades inte.");
                    }
                    else
                    {
                        throw;
                    }
                }

                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SongExists(id))
                {
                    return NotFound($"Låten med id {id} hittades inte.");
                }
                else
                {
                    _logger.LogError($"En DbUpdateConcurrencyException inträffade vid uppdatering av låt med id {id}");
                    return StatusCode(500, "Ett problem uppstod på grund av samtidig uppdatering. Försök igen.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ett fel inträffade vid uppdatering av låt med id {id}");
                return StatusCode(500, "Ett internt serverfel inträffade vid uppdatering av låten.");
            }
        }

        // POST: api/Song
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Song>> PostSong([FromBody] Song song)
        {
            try
            {
                // Om sång eller sång.Album är null, returnera BadRequest
                if (song == null)
                {
                    return BadRequest("Song är null.");
                }

                if (string.IsNullOrEmpty(song.Title) || string.IsNullOrEmpty(song.Artist))
                {
                    return BadRequest("Titel och artist måste finnas med");
                }

                // Kolla om artist och titel redan finns i databasen
                var existingSong = await _context.Songs
                    .FirstOrDefaultAsync(s => s.Title == song.Title && s.Artist == song.Artist);

                if (existingSong != null)
                {
                    return Conflict("Sången med denna artist och titel finns redan.");
                }

                // Om albumet är specificerat
                if (song.Album != null)
                {
                    if (string.IsNullOrEmpty(song.Album.Name))
                    {
                        return BadRequest("Albumnamn kan inte vara tomt.");
                    }
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
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Ett fel inträffade vid skapande av ny låt");
                return StatusCode(500, "Ett fel inträffade vid sparande av låten. Kontrollera att alla värden är korrekta.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ett oväntat fel inträffade vid skapande av ny låt");
                return StatusCode(500, "Ett internt serverfel inträffade vid skapande av låten.");
            }
        }

        // DELETE: api/Song/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSong(int id)
        {
            try
            {
                var song = await _context.Songs.FindAsync(id);
                if (song == null)
                {
                    return NotFound($"Låten med id {id} hittades inte.");
                }

                _context.Songs.Remove(song);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ett fel inträffade vid borttagning av låt med id {id}");
                return StatusCode(500, "Ett internt serverfel inträffade vid borttagning av låten.");
            }
        }

        private bool SongExists(int id)
        {
            return _context.Songs.Any(e => e.Id == id);
        }
    }
}
