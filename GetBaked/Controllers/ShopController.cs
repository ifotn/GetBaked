using GetBaked.Data;
using GetBaked.Models;
using Microsoft.AspNetCore.Mvc;

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
    }
}
