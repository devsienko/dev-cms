using Microsoft.AspNetCore.Mvc;

namespace DevCms.Controllers
{
    public class BlogController : Controller
    {
        [Route("/Blog")]
        public IActionResult Index()
        {
            return View();
        }
    }
}