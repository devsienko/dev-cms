using System;

namespace DevCms.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public NotificationStatus Status { get; set; }
        public DateTime Date { get; set; }
    }

    public enum NotificationStatus
    {
        New,
        Viewed
    }
}