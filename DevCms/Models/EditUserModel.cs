using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace DevCms.Models
{
    public class EditUserModel
    {
        [HiddenInput]
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords don't match")]
        public string ConfirmPassword { get; set; }
    }
}
