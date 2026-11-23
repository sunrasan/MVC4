using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Data;
using MvcMusicStore.Models;

namespace MvcMusicStore.ViewComponents
{
    /// <summary>
    /// Replaces the [ChildActionOnly] GenreMenu action in StoreController (issue-15).
    /// Invoked in _Layout.cshtml via @await Component.InvokeAsync("GenreMenu").
    /// </summary>
    public class GenreMenuViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public GenreMenuViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var genres = await _db.Genres
                .Include(g => g.Albums)
                    .ThenInclude(a => a.OrderDetails)
                .OrderByDescending(g => g.Albums.Sum(a => a.OrderDetails.Sum(od => od.Quantity)))
                .Take(9)
                .ToListAsync();

            return View(genres);
        }
    }
}
