using Microsoft.AspNetCore.Mvc;

namespace DevCms.Controllers
{
    public class SingleBlogController : Controller
    {
        [Route("/SingleBlog")]
        public IActionResult Index()
        {
            return View();
        }
    }
}