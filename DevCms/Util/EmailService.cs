using System.Linq;
using System.Net;
using System.Net.Mail;
using DevCms.Models;

namespace DevCms.Util
{
    public class EmailService
    {
        private readonly DevCmsDb _db;

        public EmailService(DevCmsDb db)
        {
            _db = db;
        }

        public virtual void SendEmail(Notification notification)
        {
            using (var message = new MailMessage())
            {
                message.To.Add(new MailAddress(GetMessageToAddress()));
                message.From = new MailAddress("mailbotarmatron64@gmail.com");
                message.Subject = "Вам сообщение от: " + notification.Name;
                message.Body = GetEmailBody(notification);
                message.IsBodyHtml = true;

                using (var client = new SmtpClient("smtp.gmail.com"))
                {
                    client.Port = 587;
                    client.Credentials = new NetworkCredential("mailbotarmatron64@gmail.com", "UNqVh3PA");
                    client.EnableSsl = true;
                    client.Send(message);
                }
            }
        }

        private string GetMessageToAddress()
        {
            var settings = _db.ApplicationSettings.FirstOrDefault();
            if (settings != null)
                return settings.NotificationRedirectionEmail;
            return "daniil_evsienko@mail.ru";
        }
        
        private string GetEmailBody(Notification notification)
        {
            var result = "<p>Обращение пользователя</p> <p>Дата: " + notification.Date.ToShortDateString()
                + " " + notification.Date.ToShortTimeString()
                + "</p> <p>Имя: " + notification.Name + "</p> <p>Email: " + notification.Email
                + "</p> <p>Телефон: " + notification.Phone + "</p> <p>Сообщение: " + notification.Message + "</p>";
            return result;
        }
    }
}
