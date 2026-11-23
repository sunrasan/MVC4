using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Data;
using MvcMusicStore.Models;

namespace MvcMusicStore.Controllers
{
    public class StoreController : Controller
    {
        private readonly ApplicationDbContext _db;

        public StoreController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Store/
        public async Task<IActionResult> Index()
        {
            var genres = await _db.Genres.ToListAsync();
            return View(genres);
        }

        // GET: /Store/Browse?genre=Disco
        public async Task<IActionResult> Browse(string genre)
        {
            // Replace string-based Include with lambda Include (issue-24)
            var genreModel = await _db.Genres
                .Include(g => g.Albums)
                .SingleAsync(g => g.Name == genre);

            return View(genreModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var album = await _db.Albums
                .Include(a => a.Genre)
                .Include(a => a.Artist)
                .FirstOrDefaultAsync(a => a.AlbumId == id);

            if (album == null)
                return NotFound();

            return View(album);
        }
    }
}
