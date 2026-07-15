using System.ComponentModel.DataAnnotations;
using ELMS.Commons.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ELMS.Models.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a role.")]
        public Roles? Role { get; set; }   // Selected Role
        public IEnumerable<SelectListItem> RoleList { get; set; }
        public string? EmailAddress { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
        public RegisterDto()
        {
            RoleList = Enum.GetValues(typeof(Roles))
                           .Cast<Roles>()
                           .Select(x => new SelectListItem
                           {
                               Text = x.ToString(),
                               Value = ((int)x).ToString()
                           });
        }
    }
}