using GetBaked.Data;
using GetBaked.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;

namespace GetBaked.Controllers
{
    public class ShopController : Controller
    {
        // add db connection whenever an instance of the controller is created
        private readonly ApplicationDbContext _context;

        // add configuration reader to get Stripe Key from appsettings.json
        private readonly IConfiguration _configuration;

        // constructor method - runs automatically when controller instance created
        public ShopController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
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

        // GET: /Shop/Checkout => show empty checkout page to capture customer info
        [Authorize]
        public IActionResult Checkout()
        {
            return View();
        }

        // POST: /Shop/Checkout => create Order object and store as session var before payment
        [HttpPost]
        [Authorize]
        public IActionResult Checkout([Bind("FirstName,LastName,Address,City,Province,PostalCode,Phone")] Order order)
        {
            // 7 fields bound from form inputs in method header
            // now auto-fill 3 of the fields we removed from the form
            order.OrderDate = DateTime.Now;
            order.CustomerId = User.Identity.Name;

            order.OrderTotal = (from c in _context.CartItems
                                where c.CustomerId == HttpContext.Session.GetString("CustomerId")
                                select c.Quantity * c.Price).Sum();

            // store the order as session var so we can proceed to payment attempt
            HttpContext.Session.SetObject("Order", order);

            // redirect to payment
            return RedirectToAction("Payment");
        }

        public IActionResult Payment()
        {
            // retrieve order session var (convert json string back to our Order class object
            var order = HttpContext.Session.GetObject<Order>("Order");

            // set up Stripe payment attempt
            // 1 - get Stripe API key 
            StripeConfiguration.ApiKey = _configuration.GetValue<string>("StripeSecretKey");

            // 2 - set up payment screen from Stripe API
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card"
                },
                LineItems = new List<SessionLineItemOptions> 
                { 
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long?)(order.OrderTotal * 100),
                            Currency = "cad",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "GetBaked Online Purchase"
                            }
                        },
                        Quantity = 1
                    }                 
                },
                Mode = "payment",
                SuccessUrl = "https://" + Request.Host + "/Shop/SaveOrder",
                CancelUrl = "https://" + Request.Host + "/Shop/Cart"
            };

            // 3 - invoke Stripe
            var service = new SessionService();
            Session session = service.Create(options);

            // 4 - redirect based on Stripe response
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        // GET: /Shop/SaveOrder => save order to db after payment
        public IActionResult SaveOrder()
        {
            // get the order from session var
            var order = HttpContext.Session.GetObject<Order>("Order");

            // create new order in db
            _context.Orders.Add(order);
            _context.SaveChanges();

            // copy each cartItem to a new OrderDetail as a child of the new order
            // identify which cart to fetch & display
            var customerId = GetCustomerId();

            // query the db for the cart items; include or JOIN to parent Product to get Product details
            var cartItems = _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.CustomerId == customerId);

            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    Quantity = item.Quantity,
                    Price = item.Price,
                    ProductId = item.ProductId,
                    OrderId = order.OrderId
                };

                _context.OrderDetails.Add(orderDetail);
            }
            _context.SaveChanges();

            // empty cart
            foreach (var item in cartItems)
            {
                _context.CartItems.Remove(item);
            }
            _context.SaveChanges();

            HttpContext.Session.Clear();

            // redirect to Orders/Details/5
            return RedirectToAction("Details", "Orders", new { @id = order.OrderId });

        }
    }
}
