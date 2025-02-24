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

        public AlbumController(MusicContext context)
        {
            _context = context;
        }

        // GET: api/Album
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAlbums()
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

        // GET: api/Album/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetAlbum(int id)
        {
            var album = await _context.Albums
        .Include(a => a.Songs)
        .FirstOrDefaultAsync(a => a.Id == id);

            if (album == null)
            {
                return NotFound();
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

        // PUT: api/Album/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAlbum(int id, Album album)
        {
            if (id != album.Id)
            {
                return BadRequest();
            }

            // Kontrollera om albumet finns
            var existingAlbum = await _context.Albums
                .Include(a => a.Songs)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (existingAlbum == null)
            {
                return NotFound();
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
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Album
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Album>> PostAlbum(Album album)
        {
            if (album == null || string.IsNullOrEmpty(album.Name))
            {
                return BadRequest("Albumet måste ha ett namn");
            }

            var existingAlbum = await _context.Albums
             .FirstOrDefaultAsync(a => a.Name == album.Name);

            if (existingAlbum != null)
            {
                return BadRequest("En album med detta namn finns redan");
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

        // DELETE: api/Album/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAlbum(int id)
        {
            var album = await _context.Albums
        .Include(a => a.Songs)
        .FirstOrDefaultAsync(a => a.Id == id);

            if (album == null)
            {
                return NotFound();
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

        private bool AlbumExists(int id)
        {
            return _context.Albums.Any(e => e.Id == id);
        }
    }
}
