using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Data;
using MvcMusicStore.Models;

namespace MvcMusicStore.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class StoreManagerController : Controller
    {
        private readonly ApplicationDbContext _db;

        public StoreManagerController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /StoreManager/
        public async Task<IActionResult> Index()
        {
            var albums = await _db.Albums
                .Include(a => a.Genre)
                .Include(a => a.Artist)
                .OrderBy(a => a.Price)
                .ToListAsync();
            return View(albums);
        }

        // GET: /StoreManager/Details/5
        public async Task<IActionResult> Details(int id = 0)
        {
            Album? album = await _db.Albums
                .Include(a => a.Genre)
                .Include(a => a.Artist)
                .FirstOrDefaultAsync(a => a.AlbumId == id);

            if (album == null)
                return NotFound(); // Replaces HttpNotFound() (issue-26)

            return View(album);
        }

        // GET: /StoreManager/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.GenreId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _db.Genres.ToListAsync(), "GenreId", "Name");
            ViewBag.ArtistId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _db.Artists.ToListAsync(), "ArtistId", "Name");
            return View();
        }

        // POST: /StoreManager/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Album album)
        {
            if (ModelState.IsValid)
            {
                _db.Albums.Add(album);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.GenreId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _db.Genres.ToListAsync(), "GenreId", "Name", album.GenreId);
            ViewBag.ArtistId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _db.Artists.ToListAsync(), "ArtistId", "Name", album.ArtistId);
            return View(album);
        }

        // GET: /StoreManager/Edit/5
        public async Task<IActionResult> Edit(int id = 0)
        {
            Album? album = await _db.Albums.FindAsync(id);
            if (album == null)
                return NotFound();

            ViewBag.GenreId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _db.Genres.ToListAsync(), "GenreId", "Name", album.GenreId);
            ViewBag.ArtistId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _db.Artists.ToListAsync(), "ArtistId", "Name", album.ArtistId);
            return View(album);
        }

        // POST: /StoreManager/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Album album)
        {
            if (ModelState.IsValid)
            {
                // Use _db.Update() instead of db.Entry(album).State = EntityState.Modified (issue-23)
                _db.Update(album);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.GenreId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _db.Genres.ToListAsync(), "GenreId", "Name", album.GenreId);
            ViewBag.ArtistId = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(
                await _db.Artists.ToListAsync(), "ArtistId", "Name", album.ArtistId);
            return View(album);
        }

        // GET: /StoreManager/Delete/5
        public async Task<IActionResult> Delete(int id = 0)
        {
            Album? album = await _db.Albums.FindAsync(id);
            if (album == null)
                return NotFound();

            return View(album);
        }

        // POST: /StoreManager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Album? album = await _db.Albums.FindAsync(id);
            if (album != null)
            {
                _db.Albums.Remove(album);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
        // Dispose() override removed - DI container manages DbContext lifetime (issue-36)
    }
}
