using Microsoft.AspNetCore.Mvc;

namespace DevCms.Controllers
{
    public class CandidatesController : Controller
    {
        [Route("/Candidates")]
        public IActionResult Index()
        {
            return View();
        }
    }
}