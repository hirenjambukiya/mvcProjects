namespace ex_EMSWithAJAX.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public decimal Salary { get; set; }

        public string Address { get; set; } = string.Empty;

        public int CountryId { get; set; }

        public int StateId { get; set; }

        public int CityId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
