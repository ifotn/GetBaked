using GetBaked.Data;
using GetBaked.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GetBaked.Controllers
{
    public class ShopController : Controller
    {
        // add db connection whenever an instance of the controller is created
        private readonly ApplicationDbContext _context;

        // constructor method - runs automatically when controller instance created
        public ShopController(ApplicationDbContext context)
        {
            _context = context; 
        }

        public IActionResult Index()
        {
            // use db connection to fetch then display list of categories
            var categories = _context.Categories.OrderBy(c => c.Name).ToList();

            // pass the categories to the view for display
            return View(categories);
        }

        // accept an int parameter to read the "id" value from the url
        public IActionResult ByCategory(int id)
        {
            // look up the category by the id, store name in the ViewData object for display on the page
            var category = _context.Categories.Find(id);

            // return to /shop if category not found
            if (category == null) {
                return RedirectToAction("Index");
            }

            ViewData["Category"] = category.Name;

            // use our Product model to get the products in the selected category
            var products = _context.Products.Where(p => p.CategoryId == id)
                .OrderBy(p => p.Name)
                .ToList();

            // send the products list to the view for display
            return View(products);
        }

        // POST: /Shop/AddToCart
        [HttpPost]
        public IActionResult AddToCart(int ProductId, int Quantity)
        {
            // get product so we can access the current price
            var product = _context.Products.Find(ProductId);

            // does this cart already have this product? 
            var cartItem = _context.CartItems.SingleOrDefault(c => c.ProductId == ProductId &&
                c.CustomerId == GetCustomerId());

            if (cartItem == null)
            {
                // create a new CartItem
                cartItem = new CartItem
                {
                    ProductId = ProductId,
                    Quantity = Quantity,
                    Price = product.Price,
                    CustomerId = GetCustomerId()
                };
                
                _context.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += Quantity;
                _context.Update(cartItem);
            }                

            // save to db          
            _context.SaveChanges();

            // show the cart page
            return RedirectToAction("Cart");
        }

        // identify customer to ensure unique carts
        private string GetCustomerId()
        {
            // check if we already have a session var for this user
            if (HttpContext.Session.GetString("CustomerId") == null)
            {
                // create new session var using a GUID
                HttpContext.Session.SetString("CustomerId", Guid.NewGuid().ToString()); 
            }

            // pass back the session var so we can help identify this user's cart (even when anonymous)
            return HttpContext.Session.GetString("CustomerId");
        }

        // GET: /Shop/Cart
        public IActionResult Cart()
        {
            // identify which cart to fetch & display
            var customerId = GetCustomerId();

            // query the db for the cart items; include or JOIN to parent Product to get Product details
            var cartItems = _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.CustomerId == customerId).ToList();

            // count total # of items in cart for navbar display & store in Session var
            var itemCount = (from c in cartItems
                             select c.Quantity).Sum();
            HttpContext.Session.SetInt32("ItemCount", itemCount);

            return View(cartItems);
        }

        // GET: /Shop/RemoveFromCart/5
        public IActionResult RemoveFromCart(int id)
        {
            // find the item marked for deletion
            var cartItem = _context.CartItems.Find(id);

            // perform delete
            _context.CartItems.Remove(cartItem);
            _context.SaveChanges();

            // refresh cart
            return RedirectToAction("Cart");
        }
    }
}
