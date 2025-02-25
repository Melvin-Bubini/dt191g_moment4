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
    public class AlbumController : ControllerBase
    {
        private readonly MusicContext _context;
        private readonly ILogger<AlbumController> _logger;

        public AlbumController(MusicContext context, ILogger<AlbumController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Album
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAlbums()
        {
            try
            {
                var albums = await _context.Albums
                    .Include(a => a.Songs)
                    .ToListAsync();

                var albumDtos = albums.Select(album => new
                {
                    album.Id,
                    album.Name,
                    Songs = album.Songs.Select(song => new
                    {
                        song.Id,
                        song.Artist,
                        song.Title,
                        song.Length,
                        song.Category
                    }).ToList()
                });

                return Ok(albumDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ett fel inträffade vid hämtning av alla album");
                return StatusCode(500, "Ett internt serverfel inträffade vid hämtning av album.");
            }
        }

        // GET: api/Album/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetAlbum(int id)
        {
            try
            {
                var album = await _context.Albums
                    .Include(a => a.Songs)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (album == null)
                {
                    return NotFound($"Albumet med id {id} hittades inte.");
                }

                var albumDto = new
                {
                    album.Id,
                    album.Name,
                    Songs = album.Songs.Select(song => new
                    {
                        song.Id,
                        song.Artist,
                        song.Title,
                        song.Length,
                        song.Category
                    }).ToList()
                };

                return Ok(albumDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ett fel inträffade vid hämtning av album med id {id}");
                return StatusCode(500, "Ett internt serverfel inträffade vid hämtning av albumet.");
            }
        }

        // PUT: api/Album/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAlbum(int id, Album album)
        {
            if (album == null)
            {
                return BadRequest("Albumet kan inte vara null.");
            }

            if (id != album.Id)
            {
                return BadRequest("ID i URL:en matchar inte ID i albumobjektet.");
            }

            if (string.IsNullOrEmpty(album.Name))
            {
                return BadRequest("Albumet måste ha ett namn.");
            }

            try
            {
                // Kontrollera om albumet finns
                var existingAlbum = await _context.Albums
                    .Include(a => a.Songs)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (existingAlbum == null)
                {
                    return NotFound($"Albumet med id {id} hittades inte.");
                }

                // Kontrollera om namnet redan används av ett annat album
                var duplicateAlbum = await _context.Albums
                    .FirstOrDefaultAsync(a => a.Id != id && a.Name == album.Name);

                if (duplicateAlbum != null)
                {
                    return Conflict($"Ett album med namnet '{album.Name}' finns redan.");
                }

                // Uppdatera bara namnet, inte relationen till låtar
                existingAlbum.Name = album.Name;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlbumExists(id))
                    {
                        return NotFound($"Albumet med id {id} hittades inte.");
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
                _logger.LogError($"En DbUpdateConcurrencyException inträffade vid uppdatering av album med id {id}");
                return StatusCode(500, "Ett problem uppstod på grund av samtidig uppdatering. Försök igen.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ett fel inträffade vid uppdatering av album med id {id}");
                return StatusCode(500, "Ett internt serverfel inträffade vid uppdatering av albumet.");
            }
        }

        // POST: api/Album
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Album>> PostAlbum(Album album)
        {
            try
            {
                if (album == null || string.IsNullOrEmpty(album.Name))
                {
                    return BadRequest("Albumet måste ha ett namn.");
                }

                var existingAlbum = await _context.Albums
                    .FirstOrDefaultAsync(a => a.Name == album.Name);

                if (existingAlbum != null)
                {
                    return Conflict($"Ett album med namnet '{album.Name}' finns redan.");
                }

                _context.Albums.Add(album);
                await _context.SaveChangesAsync();

                var albumDto = new
                {
                    album.Id,
                    album.Name,
                    Songs = new List<object>() // Tom lista eftersom ett nytt album inte har några låtar än
                };

                return CreatedAtAction("GetAlbum", new { id = album.Id }, albumDto);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Ett fel inträffade vid skapande av nytt album");
                return StatusCode(500, "Ett fel inträffade vid sparande av albumet. Kontrollera att alla värden är korrekta.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ett oväntat fel inträffade vid skapande av nytt album");
                return StatusCode(500, "Ett internt serverfel inträffade vid skapande av albumet.");
            }
        }

        // DELETE: api/Album/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlbum(int id)
        {
            try
            {
                var album = await _context.Albums
                    .Include(a => a.Songs)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (album == null)
                {
                    return NotFound($"Albumet med id {id} hittades inte.");
                }

                // Alternativ 1: Ta bort albumreferensen från låtarna men behåll låtarna
                foreach (var song in album.Songs.ToList())
                {
                    song.AlbumId = null;
                    song.Album = null;
                }

                _context.Albums.Remove(album);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Ett fel inträffade vid borttagning av album med id {id}");
                return StatusCode(500, "Ett fel inträffade vid borttagning av albumet. Det kan finnas beroenden som inte kunde hanteras.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ett oväntat fel inträffade vid borttagning av album med id {id}");
                return StatusCode(500, "Ett internt serverfel inträffade vid borttagning av albumet.");
            }
        }

        private bool AlbumExists(int id)
        {
            return _context.Albums.Any(e => e.Id == id);
        }
    }
}
