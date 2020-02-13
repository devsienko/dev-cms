using Microsoft.AspNetCore.Mvc;

namespace DevCms.Controllers
{
    public class ContactsController : Controller
    {
        [Route("/Contacts")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
