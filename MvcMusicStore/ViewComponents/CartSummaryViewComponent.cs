using Microsoft.AspNetCore.Mvc;
using MvcMusicStore.Data;
using MvcMusicStore.Models;

namespace MvcMusicStore.ViewComponents
{
    /// <summary>
    /// Replaces the [ChildActionOnly] CartSummary action in ShoppingCartController (issue-14).
    /// Invoked in _Layout.cshtml via @await Component.InvokeAsync("CartSummary").
    /// </summary>
    public class CartSummaryViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;

        public CartSummaryViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public IViewComponentResult Invoke()
        {
            var cart = ShoppingCart.GetCart(_db, HttpContext);

            var cartItems = cart.GetCartItems()
                .Select(a => a.Album?.Title ?? string.Empty)
                .OrderBy(x => x);

            ViewBag.CartCount = cartItems.Count();
            ViewBag.CartSummary = string.Join("\n", cartItems.Distinct());

            return View();
        }
    }
}
