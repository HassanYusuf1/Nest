using Microsoft.AspNetCore.Mvc;

namespace Instagram.Controllers
{
    public class HomeController : Controller
    {
        
        public IActionResult Index()
        {
            
            if (User?.Identity?.IsAuthenticated == true)
            {
              
                return Redirect("~/Picture/Home");
            }

          
            return View();
        }
    }
}
