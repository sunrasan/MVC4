using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMusicStore.Data;
using MvcMusicStore.Models;

namespace MvcMusicStore.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _db;
        private const string PromoCode = "FREE";

        public CheckoutController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Checkout/
        public IActionResult AddressAndPayment()
        {
            return View();
        }

        // POST: /Checkout/AddressAndPayment
        // Replaced FormCollection + TryUpdateModel with strongly-typed model binding (issue-13)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddressAndPayment(Order order, string? promoCode)
        {
            if (!ModelState.IsValid)
                return View(order);

            if (!string.Equals(promoCode, PromoCode, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("PromoCode", "Invalid promo code.");
                return View(order);
            }

            try
            {
                order.Username = User.Identity?.Name ?? string.Empty;
                order.OrderDate = DateTime.Now;

                _db.Orders.Add(order);

                var cart = ShoppingCart.GetCart(_db, HttpContext);
                cart.CreateOrder(order);

                await _db.SaveChangesAsync();

                return RedirectToAction("Complete", new { id = order.OrderId });
            }
            catch
            {
                return View(order);
            }
        }

        // GET: /Checkout/Complete
        public async Task<IActionResult> Complete(int id)
        {
            bool isValid = await _db.Orders.AnyAsync(
                o => o.OrderId == id && o.Username == User.Identity!.Name);

            if (isValid)
                return View(id);
            else
                return View("Error");
        }
    }
}
