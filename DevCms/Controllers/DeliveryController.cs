using Microsoft.AspNetCore.Mvc;

namespace DevCms.Controllers
{
    public class DeliveryController : Controller
    {
        [Route("/Delivery")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
