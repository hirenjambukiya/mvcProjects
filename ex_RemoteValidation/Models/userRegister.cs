using ex_RemoteValidation.CustomValidation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


namespace ex_RemoteValidation.Models
{
    public class userRegister
    {
        [Required]
        [Remote("IsUsernameAvailable", "Account")]
        [NoSpecialCharactersAttribute(ErrorMessage = "No special characters allowed.")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Remote("IsEmailAvailable", "Account")]
        public string Email { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
