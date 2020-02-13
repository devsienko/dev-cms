using System.ComponentModel.DataAnnotations;

namespace DevCms.Models
{
    public class SettingsDto
    {
        [Required(ErrorMessage = @"Field ""Feedback email"" is required.")]
        [EmailAddress(ErrorMessage = "Incorrect email.")]
        [DataType(DataType.EmailAddress)]
        public string NotificationRedirectionEmail { get; set; }
    }
}