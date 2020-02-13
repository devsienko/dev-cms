using System;
using System.ComponentModel.DataAnnotations;

namespace DevCms.Db
{
    public class User
    {
        public int Id { get; set; }

        [StringLength(255)]
        public string Email { get; set; }

        [StringLength(255)]
        public string Password { get; set; }

        [StringLength(255)]
        public string PasswordSalt { get; set; }

        [StringLength(255)]
        public string RegistrationStatus { get; set; }

        [StringLength(255)]
        public string EmailStatus { get; set; }

        [StringLength(255)]
        public string SecurityQuestion { get; set; }

        [StringLength(255)]
        public string SecurityAnswer { get; set; }

        public DateTime? RegistrationDateTime { get; set; }

        public DateTime? FailedLoginAttemptDateTime { get; set; }

        public int FailedLoginAttemptCounter { get; set; }
    }
}
