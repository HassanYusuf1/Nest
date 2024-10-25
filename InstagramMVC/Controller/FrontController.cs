using Microsoft.AspNetCore.Mvc;

namespace Instagram.Controllers
{
    public class FrontController : Controller
    {
        // GET: /Front/Index
        public IActionResult Index()
        {
            // Returnerer Hjem-siden (Index) som inneholder velkomst
            return View();
        }
    }
}
