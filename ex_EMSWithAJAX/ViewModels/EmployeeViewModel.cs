using System.ComponentModel.DataAnnotations;
using ex_EMSWithAJAX.Validation;

namespace ex_EMSWithAJAX.ViewModels
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select gender.")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email.")]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required.")]
        [MinimumAgeAttribute(18)]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Salary is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "Salary must be greater than 0.")]
        public decimal? Salary { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select country.")]
        public int? CountryId { get; set; }

        [Required(ErrorMessage = "Please select state.")]
        public int? StateId { get; set; }

        [Required(ErrorMessage = "Please select city.")]
        public int? CityId { get; set; }

        public IEnumerable<Models.Country> Countries { get; set; }
            = new List<Models.Country>();

        public IEnumerable<Models.State> States { get; set; }
            = new List<Models.State>();

        public IEnumerable<Models.City> Cities { get; set; }
            = new List<Models.City>();

        public bool IsDateOfBirthValid()
        {
            if (!DateOfBirth.HasValue)
                return false;

            if (DateOfBirth.Value.Date <=DateTime.Today)
            {
                return false;
            }
            int diff = DateTime.Now.Year - DateOfBirth.Value.Year;
            if (diff<18)
            {
                return false;
            }
            return true;
        }
    }
}
