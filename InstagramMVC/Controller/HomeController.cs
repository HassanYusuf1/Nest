using Microsoft.AspNetCore.Mvc;

namespace Instagram.Controllers
{
    public class HomeController : Controller
    {
        // GET: /Home/Index
        public IActionResult Index()
        {
            // Check if the User object and Identity are not null, and if the user is logged in
            if (User?.Identity?.IsAuthenticated == true)
            {
                // If logged in, redirect to the user's main page
                return Redirect("~/Picture/MyPage"); 
            }

            // Show the default welcome page for users who aren’t logged in
            return View();
        }
    }
}
