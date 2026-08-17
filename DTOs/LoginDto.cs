// DTOs/LoginDto.cs
using System.ComponentModel.DataAnnotations;

namespace EventTicketingApp.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "A valid email address is required.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
    }
}