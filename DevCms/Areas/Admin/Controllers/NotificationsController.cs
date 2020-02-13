using System.Linq;
using Microsoft.AspNetCore.Mvc;
using DevCms.Models;
using Microsoft.AspNetCore.Authorization;

namespace DevCms.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class NotificationsController : Controller
    {
        private readonly DevCmsDb _context;

        public NotificationsController(DevCmsDb context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new NotificationView();
            var allNotifications = _context.Notifications
                .OrderByDescending(n => n.Date)
                .ToList();
            model.New = allNotifications.Where(n => n.Status == NotificationStatus.New).ToList();
            model.Viewed = allNotifications.Where(n => n.Status == NotificationStatus.Viewed).ToList();

            return View(model);
        }

        public IActionResult Notification(int? id)
        {
            if (id == null || id < 1)
            {
                return NotFound();
            }

            var model = _context.Notifications.FirstOrDefault(n => n.Id == id);
            if (model == null)
            {
                return NotFound();
            }

            if (model.Status == NotificationStatus.New)
            {
                model.Status = NotificationStatus.Viewed;
                _context.SaveChanges();
            }

            return View(model);
        }
    }
}
