using Microsoft.AspNetCore.Mvc;

namespace GetBaked.Controllers
{
    public class ProductsController : Controller
    {
        // GET: /products/create 
        public IActionResult Create()
        {
            return View();
        }

        // GET: /products/edit/5
        public IActionResult Edit(int id) 
        { 
            return View();
        }
    }
}
