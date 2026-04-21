using System.ComponentModel.DataAnnotations;

namespace ex_ConceptOfMVC.Models.Authontication
{
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Role { get; set; } = "User";

        public int FailedLoginAttempts { get; set; } = 0;

        public bool IsLockedOut { get; set; } = false;

        public DateTime? LockoutEnd { get; set; }
    }
}
