using Microsoft.AspNetCore.Mvc;

namespace DevCms.Controllers
{
    public class JobsController : Controller
    {
        [Route("/Jobs")]
        public IActionResult Index()
        {
            return View();
        }
    }
}