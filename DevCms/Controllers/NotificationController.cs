using System;
using Microsoft.AspNetCore.Mvc;
using DevCms.Models;
using DevCms.Util;

namespace DevCms.Controllers
{
    public class NotificationController : Controller
    {
        private readonly DevCmsDb _db;
        private readonly EmailService _emailService; 

        public NotificationController(DevCmsDb db, EmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }

        [HttpPost]
        public string Create(Notification notification)
        {
            try
            {
                if (notification == null || string.IsNullOrEmpty(notification.Email)
                                         || string.IsNullOrEmpty(notification.Name)
                                         || string.IsNullOrEmpty(notification.Message))
                {
                    return "Error. Incorrect form data.";
                }
                notification.Date = DateTime.UtcNow.AddHours(4);//Saratov time;
                _db.Notifications.Add(notification);
                _db.SaveChanges();
                _emailService.SendEmail(notification);
                return "success";
            }
            catch (Exception ex)
            {
                //todo: use logger
                return "Error." + ex.Message;
            }
        }
    }
}
