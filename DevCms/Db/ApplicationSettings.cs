using System.ComponentModel.DataAnnotations;

namespace DevCms.Db
{
    public class ApplicationSettings
    {
        public int Id { get; set; }

        [StringLength(255)]
        public string NotificationRedirectionEmail { get; set; }
    }
}
