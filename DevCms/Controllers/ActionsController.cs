using Microsoft.AspNetCore.Mvc;

namespace DevCms.Controllers
{
    public class ActionsController : Controller
    {
        [Route("/Actions")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
