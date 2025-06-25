using System.ComponentModel.DataAnnotations;

namespace ex_TableDataDashboard.Models
{
    public class SqlLoginViewModel
    {
        [Required]
        public string Server { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Password { get; set; }
    }

}
