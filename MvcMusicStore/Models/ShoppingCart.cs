using MvcMusicStore.Data;

namespace MvcMusicStore.Models
{
    /// <summary>
    /// ShoppingCart model - migrated from System.Web.HttpContextBase to
    /// ASP.NET Core HttpContext with ISession (GetString/SetString).
    /// </summary>
    public partial class ShoppingCart
    {
        private readonly ApplicationDbContext _db;
        private string ShoppingCartId { get; set; } = string.Empty;

        public ShoppingCart(ApplicationDbContext db)
        {
            _db = db;
        }

        public const string CartSessionKey = "CartId";

        /// <summary>
        /// Factory method using ASP.NET Core HttpContext.
        /// </summary>
        public static ShoppingCart GetCart(ApplicationDbContext db, HttpContext context)
        {
            var cart = new ShoppingCart(db);
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }

        public void AddToCart(Album album)
        {
            var cartItem = _db.Carts.SingleOrDefault(
                c => c.CartId == ShoppingCartId && c.AlbumId == album.AlbumId);

            if (cartItem == null)
            {
                cartItem = new Cart
                {
                    AlbumId = album.AlbumId,
                    CartId = ShoppingCartId,
                    Count = 1,
                    DateCreated = DateTime.Now
                };
                _db.Carts.Add(cartItem);
            }
            else
            {
                cartItem.Count++;
            }
        }

        public int RemoveFromCart(int id)
        {
            var cartItem = _db.Carts.Single(
                cart => cart.CartId == ShoppingCartId && cart.RecordId == id);

            int itemCount = 0;

            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                    itemCount = cartItem.Count;
                }
                else
                {
                    _db.Carts.Remove(cartItem);
                }
            }

            return itemCount;
        }

        public void EmptyCart()
        {
            var cartItems = _db.Carts.Where(cart => cart.CartId == ShoppingCartId);
            foreach (var cartItem in cartItems)
            {
                _db.Carts.Remove(cartItem);
            }
        }

        public List<Cart> GetCartItems()
        {
            return _db.Carts
                .Where(cart => cart.CartId == ShoppingCartId)
                .ToList();
        }

        public int GetCount()
        {
            int? count = (from cartItems in _db.Carts
                          where cartItems.CartId == ShoppingCartId
                          select (int?)cartItems.Count).Sum();
            return count ?? 0;
        }

        public decimal GetTotal()
        {
            decimal? total = (from cartItems in _db.Carts
                              where cartItems.CartId == ShoppingCartId
                              select (int?)cartItems.Count * cartItems.Album.Price).Sum();
            return total ?? decimal.Zero;
        }

        public int CreateOrder(Order order)
        {
            decimal orderTotal = 0;
            var cartItems = GetCartItems();

            foreach (var item in cartItems)
            {
                var album = _db.Albums.Find(item.AlbumId);
                if (album == null) continue;

                var orderDetail = new OrderDetail
                {
                    AlbumId = item.AlbumId,
                    OrderId = order.OrderId,
                    UnitPrice = album.Price,
                    Quantity = item.Count,
                };

                orderTotal += item.Count * item.Album.Price;
                _db.OrderDetails.Add(orderDetail);
            }

            order.Total = orderTotal;
            EmptyCart();
            return order.OrderId;
        }

        /// <summary>
        /// Gets or creates the cart ID using ASP.NET Core session (GetString/SetString).
        /// </summary>
        public string GetCartId(HttpContext context)
        {
            var cartId = context.Session.GetString(CartSessionKey);
            if (cartId == null)
            {
                if (!string.IsNullOrWhiteSpace(context.User.Identity?.Name))
                {
                    cartId = context.User.Identity.Name;
                }
                else
                {
                    cartId = Guid.NewGuid().ToString();
                }
                context.Session.SetString(CartSessionKey, cartId);
            }
            return cartId;
        }

        /// <summary>
        /// Migrates cart items to the logged-in user's account.
        /// </summary>
        public void MigrateCart(string userName)
        {
            var shoppingCart = _db.Carts.Where(c => c.CartId == ShoppingCartId);
            foreach (Cart item in shoppingCart)
            {
                item.CartId = userName;
            }
        }
    }
}
