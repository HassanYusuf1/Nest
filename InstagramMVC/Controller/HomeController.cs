using Microsoft.AspNetCore.Mvc;

namespace Instagram.Controllers
{
    public class HomeController : Controller
    {
        // GET: /Home/Index
        public IActionResult Index()
        {
            // Returnerer Hjem-siden (Index) som inneholder velkomst
            return View();
        }
    }
}
