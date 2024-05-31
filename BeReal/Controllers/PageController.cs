using Microsoft.AspNetCore.Mvc;

namespace BeReal.Controllers
{
    public class PageController : Controller
    {
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
