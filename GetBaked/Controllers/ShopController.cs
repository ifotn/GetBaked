using GetBaked.Models;
using Microsoft.AspNetCore.Mvc;

namespace GetBaked.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // accept a string parameter to read the "name" value from the url
        public IActionResult ByCategory(string name)
        {
            // store the selected category name in the ViewData object for display on the page
            ViewData["Category"] = name;

            // use our Product model to create an in-memory list of products
            // this will be replaced with a database query in week 4
            // first define an empty list of Product objects
            var products = new List<Product>();

            // use a loop to create some products and add each 1 to the list
            for (int i = 1; i <=10; i++)
            {
                products.Add(new Product { ProductId = i, Name = "Product " + i.ToString(), Price = 8 });
            }

            // send the products list to the view for display
            return View(products);
        }
    }
}
