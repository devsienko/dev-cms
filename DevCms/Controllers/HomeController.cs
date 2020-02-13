using DevCms.Models;
using DevCms.Util;
using Microsoft.AspNetCore.Mvc;

namespace DevCms.Controllers
{
    public class HomeController : Controller
    {
        private readonly ContentManager _contentManager;

        public HomeController(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        [Route("/")]
        public IActionResult Index()
        {
            return View(_contentManager);
        }

        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode.HasValue)
            {
                if (statusCode.Value == 404 || statusCode.Value == 500)
                {
                    var viewName = statusCode.ToString();
                    return View(viewName);
                }
            }
            return View();
        }
    }
}
