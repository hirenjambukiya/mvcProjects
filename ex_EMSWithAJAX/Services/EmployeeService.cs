using ex_EMSWithAJAX.Models;
using ex_EMSWithAJAX.Repositories;

namespace ex_EMSWithAJAX.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public IEnumerable<Employee> GetAll()
        {
            return _employeeRepository.GetAll();
        }

        public Employee? GetById(int employeeId)
        {
            return _employeeRepository.GetById(employeeId);
        }

        public bool CheckEmail(string email, int employeeId = 0)
        {
            return _employeeRepository.CheckEmail(email, employeeId);
        }

        public int Insert(Employee employee)
        {
            return _employeeRepository.Insert(employee);
        }

        public bool Update(Employee employee)
        {
            return _employeeRepository.Update(employee);
        }

        public bool Delete(int employeeId)
        {
            return _employeeRepository.Delete(employeeId);
        }

        public IEnumerable<Country> GetCountries()
        {
            return _employeeRepository.GetCountries();
        }

        public IEnumerable<State> GetStatesByCountry(int countryId)
        {
            return _employeeRepository.GetStatesByCountry(countryId);
        }

        public IEnumerable<City> GetCitiesByState(int stateId)
        {
            return _employeeRepository.GetCitiesByState(stateId);
        }
    }
}
