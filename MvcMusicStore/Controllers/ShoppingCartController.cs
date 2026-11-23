using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Data;
using MvcMusicStore.Models;
using MvcMusicStore.ViewModels;

namespace MvcMusicStore.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ShoppingCartController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /ShoppingCart/
        public async Task<IActionResult> Index()
        {
            var cart = ShoppingCart.GetCart(_db, HttpContext);

            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()
            };

            return View(viewModel);
        }

        // GET: /ShoppingCart/AddToCart/5
        public async Task<IActionResult> AddToCart(int id)
        {
            var addedAlbum = await _db.Albums.SingleAsync(album => album.AlbumId == id);

            var cart = ShoppingCart.GetCart(_db, HttpContext);
            cart.AddToCart(addedAlbum);

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // AJAX: /ShoppingCart/RemoveFromCart/5
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var cart = ShoppingCart.GetCart(_db, HttpContext);

            string albumName = (await _db.Carts
                .Include(c => c.Album)
                .SingleAsync(item => item.RecordId == id)).Album.Title;

            int itemCount = cart.RemoveFromCart(id);
            await _db.SaveChangesAsync();

            string removed = (itemCount > 0) ? " 1 copy of " : string.Empty;

            var results = new ShoppingCartRemoveViewModel
            {
                Message = removed + albumName + " has been removed from your shopping cart.",
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                ItemCount = itemCount,
                DeleteId = id
            };

            return Json(results);
        }
        // CartSummary [ChildActionOnly] removed - replaced by CartSummaryViewComponent (issue-14)
    }
}
