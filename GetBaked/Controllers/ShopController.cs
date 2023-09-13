using Microsoft.AspNetCore.Mvc;

namespace GetBaked.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
