using Microsoft.AspNetCore.Mvc;

namespace DevCms.Controllers
{
    public class JobDetailsController : Controller
    {
        [Route("/JobDetails")]
        public IActionResult Index()
        {
            return View();
        }
    }
}