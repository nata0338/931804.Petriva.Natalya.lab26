using System.ComponentModel.DataAnnotations;


namespace lab6.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required(ErrorMessage = "The Confirm Password field is required.")]
        [Compare(nameof(Password), ErrorMessage = "Passwords doesn't match the original.")]
        public string ConfirmPassword { get; set; }
    }
}
