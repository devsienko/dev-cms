using System.Collections.Generic;

namespace DevCms.Models
{
    public class NotificationView
    {
        public List<Notification> New { get; set; }
        public List<Notification> Viewed { get; set; }
    }
}