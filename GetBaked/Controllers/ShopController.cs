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
            return View();
        }
    }
}
