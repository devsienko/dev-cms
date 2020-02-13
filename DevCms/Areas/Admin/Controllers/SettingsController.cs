using System.Linq;
using DevCms.Db;
using Microsoft.AspNetCore.Mvc;
using DevCms.Models;
using Microsoft.AspNetCore.Authorization;

namespace DevCms.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class SettingsController : Controller
    {
        private readonly DevCmsDb _db;

        public SettingsController(DevCmsDb context)
        {
            _db = context;
        }

        public IActionResult Index()
        {
            var settings = _db.ApplicationSettings.FirstOrDefault();
            if (settings != null)
            {
                var model = new SettingsDto
                {
                    NotificationRedirectionEmail = settings.NotificationRedirectionEmail
                };
                return View(model);
            }

            return View(new SettingsDto());
        }

        [HttpPost]
        public IActionResult Index(SettingsDto settingsDto)
        {
            if (ModelState.IsValid)
            {
                var settings = _db.ApplicationSettings.FirstOrDefault();
                if (settings != null)
                    settings.NotificationRedirectionEmail = settingsDto.NotificationRedirectionEmail;
                else
                    _db.ApplicationSettings.Add(new ApplicationSettings
                    {
                        NotificationRedirectionEmail = settingsDto.NotificationRedirectionEmail
                    });
                _db.SaveChanges();
            }
            return View(settingsDto);
        }
    }
}
